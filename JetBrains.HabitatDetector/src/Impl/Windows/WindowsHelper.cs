using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static class WindowsHelper
  {
    internal static unsafe JetArchitecture GetProcessArchitecture()
    {
      SYSTEM_INFO systemInfo;
      Kernel32Dll.GetSystemInfo(&systemInfo);
      return ConvertToArchitecture(systemInfo.wProcessorArchitecture);
    }

    internal unsafe delegate TResult OpenProcessDelegate<out TResult>(void* hProcess);

    internal static unsafe JetArchitecture OpenProcess(int processId, OpenProcessDelegate<JetArchitecture> action)
    {
      var hProcess = Kernel32Dll.OpenProcess((uint)ProcessAccessRights.PROCESS_QUERY_LIMITED_INFORMATION, 0, unchecked((uint)processId));
      if (hProcess == null)
        throw new Win32Exception();
      try
      {
        return action(hProcess);
      }
      finally
      {
        Kernel32Dll.CloseHandle(hProcess);
      }
    }

    internal static unsafe JetArchitecture GetProcessArchitecture(void* hProcess)
    {
      if (WinApiCalls.GetProcessInformation != null)
      {
        PROCESS_MACHINE_INFORMATION pmi;
        if (WinApiCalls.GetProcessInformation(hProcess, PROCESS_INFORMATION_CLASS.ProcessMachineTypeInfo, &pmi, (uint)sizeof(PROCESS_MACHINE_INFORMATION)) == 0)
        {
          // Note(ww898): PROCESS_INFORMATION_CLASS.ProcessMachineTypeInfo can be not yet implemented. Available since Windows 10.0 Build 22000.
          // Bug(ww898): System.Runtime.InteropServices.Marshal.GetLastWin32Error() under Mono always return 127 here, because no `SetLastError = true` for delegates instead of `DllImportAttribute`!!!
          var error = Kernel32Dll.GetLastError();
          if (error != WinError.ERROR_INVALID_PARAMETER)
            throw new Win32Exception(error);
        }
        else
          return ConvertToArchitecture(pmi.ProcessMachine);
      }

      if (WinApiCalls.IsWow64Process2 != null)
      {
        // Bug(ww898): We can't detect X64 processes on ARM64 OS here!!!
        IMAGE_FILE_MACHINE processImageFileMachine, nativeImageFileMachine;
        // Bug(ww898): System.Runtime.InteropServices.Marshal.GetLastWin32Error() under Mono always return 127 here, because no `SetLastError = true` for delegates instead of `DllImportAttribute`!!!
        if (WinApiCalls.IsWow64Process2(hProcess, &processImageFileMachine, &nativeImageFileMachine) == 0)
          throw new Win32Exception(Kernel32Dll.GetLastError());
        return ConvertToArchitecture(processImageFileMachine == IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_UNKNOWN
          ? nativeImageFileMachine
          : processImageFileMachine);
      }

      if (WinApiCalls.IsWow64Process != null)
      {
        int isWow64;
        // Bug(ww898): System.Runtime.InteropServices.Marshal.GetLastWin32Error() under Mono always return 127 here, because no `SetLastError = true` for delegates instead of `DllImportAttribute`!!!
        if (WinApiCalls.IsWow64Process(hProcess, &isWow64) == 0)
          throw new Win32Exception(Kernel32Dll.GetLastError());
        if (isWow64 != 0)
          return JetArchitecture.X86;

        SYSTEM_INFO nativeSystemInfo;
        Kernel32Dll.GetNativeSystemInfo(&nativeSystemInfo);
        return ConvertToArchitecture(nativeSystemInfo.wProcessorArchitecture);
      }

      SYSTEM_INFO systemInfo;
      Kernel32Dll.GetSystemInfo(&systemInfo);
      return ConvertToArchitecture(systemInfo.wProcessorArchitecture);
    }

    internal static unsafe JetArchitecture GetOSArchitecture()
    {
      if (WinApiCalls.IsWow64Process2 != null)
      {
        IMAGE_FILE_MACHINE processImageFileMachine, nativeImageFileMachine;
        if (WinApiCalls.IsWow64Process2(Kernel32Dll.GetCurrentProcess(), &processImageFileMachine, &nativeImageFileMachine) == 0)
          throw new Win32Exception();
        return ConvertToArchitecture(nativeImageFileMachine);
      }

      // Note(ww898): GetNativeSystemInfo() on Windows ARM64 returns 9(PROCESSOR_ARCHITECTURE_AMD64) instead of 12(PROCESSOR_ARCHITECTURE_ARM64) for X64 processes!!!
      SYSTEM_INFO nativeSystemInfo;
      Kernel32Dll.GetNativeSystemInfo(&nativeSystemInfo);
      return ConvertToArchitecture(nativeSystemInfo.wProcessorArchitecture);
    }

    internal static unsafe ElevationData GetElevation() => GetElevation(Kernel32Dll.GetCurrentProcess());

    internal static unsafe ElevationData GetElevation(void* hProcess)
    {
      void* hToken;
      if (Advapi32Dll.OpenProcessToken(hProcess, (uint)(TokenSpecificAccessRights.TOKEN_DUPLICATE | TokenSpecificAccessRights.TOKEN_QUERY), &hToken) == 0)
        throw new Win32Exception();
      try
      {
        TOKEN_ELEVATION te;
        var teSize = (uint)sizeof(TOKEN_ELEVATION);
        if (Advapi32Dll.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevation, &te, teSize, &teSize) == 0)
          throw new Win32Exception();

        TOKEN_ELEVATION_TYPE tet;
        var tetSize = (uint)sizeof(TOKEN_ELEVATION_TYPE);
        if (Advapi32Dll.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevationType, &tet, tetSize, &tetSize) == 0)
          throw new Win32Exception();

        void* hImpersonationToken;
        if (Advapi32Dll.DuplicateToken(hToken, SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, &hImpersonationToken) == 0)
          throw new Win32Exception();
        try
        {
          return new(
            IsBuildInGroupMember(hImpersonationToken, RID.DOMAIN_ALIAS_RID_ADMINS),
            te.TokenIsElevated != 0,
            tet switch
              {
                TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault => JetWindowsTokenElevationType.Default,
                TOKEN_ELEVATION_TYPE.TokenElevationTypeFull => JetWindowsTokenElevationType.Full,
                TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited => JetWindowsTokenElevationType.Limited,
                _ => throw new ArgumentOutOfRangeException()
              });
        }
        finally
        {
          Kernel32Dll.CloseHandle(hImpersonationToken);
        }
      }
      finally
      {
        Kernel32Dll.CloseHandle(hToken);
      }
    }

    private static unsafe bool IsBuildInGroupMember(void* hToken, uint domainAliasRid)
    {
      var ntAuthority = SID_IDENTIFIER_AUTHORITY.SECURITY_NT_AUTHORITY;
      void* groupSid;
      if (Advapi32Dll.AllocateAndInitializeSid(
            &ntAuthority,
            2,
            RID.SECURITY_BUILTIN_DOMAIN_RID,
            domainAliasRid,
            0, 0, 0, 0, 0, 0,
            &groupSid) == 0)
        throw new Win32Exception();
      try
      {
        int isMember;
        if (Advapi32Dll.CheckTokenMembership(hToken, groupSid, &isMember) == 0)
          throw new Win32Exception();
        return isMember != 0;
      }
      finally
      {
        Advapi32Dll.FreeSid(groupSid);
      }
    }

    private static JetArchitecture ConvertToArchitecture(PROCESSOR_ARCHITECTURE processorArchitecture) => processorArchitecture switch
      {
        PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_INTEL => JetArchitecture.X86,
        PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_AMD64 => JetArchitecture.X64,
        PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM => JetArchitecture.Arm,
        PROCESSOR_ARCHITECTURE.PROCESSOR_ARCHITECTURE_ARM64 => JetArchitecture.Arm64,
        _ => throw new PlatformNotSupportedException($"Unsupported processor architecture {processorArchitecture}")
      };

    private static JetArchitecture ConvertToArchitecture(IMAGE_FILE_MACHINE imageFileMachine) => imageFileMachine switch
      {
        IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_I386 => JetArchitecture.X86,
        IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_AMD64 => JetArchitecture.X64,
        IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARMNT => JetArchitecture.Arm,
        IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARM64 => JetArchitecture.Arm64,
        _ => throw new PlatformNotSupportedException($"Unsupported machine identifier {imageFileMachine}")
      };

    internal record struct OSInfo(string OSName, uint BuildNumber, JetWindowsInstallationType? InstallationType);

    internal static string FixProductName(string productName, uint buildNumber)
    {
      // Windows 10:
      //   | Build | Workstation | Server    |
      //   +=======+=============+===========+
      //   | 10240 |   10 1507   | --------- |
      //   | 10586 |      1511   | --------- |
      //   | 14393 |      1607   | 2016 1607 |
      //   | 15063 |      1703   | --------- |
      //   | 16299 |      1709   |      1709 |
      //   | 17134 |      1803   |      1803 |
      //   | 17763 |      1809   | 2019 1809 |
      //   | 18362 |      1903   |      1903 |
      //   | 18363 |      1909   |      1909 |
      //   | 19041 |      2004   |      2004 |
      //   | 19042 |      20H2   |      20H2 |
      //   | 19043 |      21H1   | --------- |
      //   | 19044 |      21H2   | --------- |
      //   | 19045 |      22H2   | --------- |
      //   | 20348 | ----------- | 2022 21H2 |
      //   | 22000 |   11 21H2   | --------- |
      //   | 22621 |      22H2   | --------- |
      //   | 22631 |      23H2   | --------- |

      // See https://en.wikipedia.org/wiki/List_of_Microsoft_Windows_versions

      // Note(ww898): The `product_name` for Windows Vista has `(TM)` in name, so remove it!
      productName = productName.Replace(" (TM)", "");

      // Note(ww898): Fix OS name because we want to avoid COM WMI calls!!!
      if (productName.IndexOf(" Server", StringComparison.Ordinal) != -1)
      {
        if (buildNumber >= 14393 && !Regex.IsMatch(productName, @"Server \d+"))
          productName = productName.Replace("Server", buildNumber >= 20348 ? "Server 2022" : buildNumber >= 17763 ? "Server 2019" : "Server 2016");
      }
      else
      {
        if (buildNumber >= 22000)
          productName = productName.Replace("Windows 10", "Windows 11");

        if (buildNumber >= 10240 && !Regex.IsMatch(productName, @"Windows \d+"))
          productName = productName.Replace("Windows", buildNumber >= 22000 ? "Windows 11" : "Windows 10");
      }

      return productName;
    }

    internal static unsafe OSInfo GetOSInfo()
    {
      // Note(ww898): 32-bits processes should access 64-bits registry because correct data only in it! See https://learn.microsoft.com/en-us/windows/win32/sysinfo/registry-key-security-and-access-rights
      return RegQueryValues(HKEY.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", KeyAccessRights.KEY_WOW64_64KEY, query =>
        {
          var productName = query("ProductName").AsString();
          var buildNumber = uint.Parse(query("CurrentBuild").AsString());
          var installationType = query("InstallationType").AsOptionalString() switch
            {
              "Client" => JetWindowsInstallationType.Client,
              "Nano Server" => JetWindowsInstallationType.NanoServer,
              "Server Core" => JetWindowsInstallationType.ServerCore,
              "Server" => JetWindowsInstallationType.Server,
              "IoTCore" => JetWindowsInstallationType.IoTCore,
              _ => (JetWindowsInstallationType?)null
            };
          var ubr = query("UBR").AsOptionalDWord();
          var displayVersion = query("DisplayVersion").AsOptionalString();
          var releaseId = query("ReleaseId").AsOptionalString();

          var builder = new StringBuilder(FixProductName(productName, buildNumber));
          if (displayVersion != null)
            builder.Append(' ').Append(displayVersion);
          else if (releaseId != null)
            builder.Append(' ').Append(releaseId);

          // Note(ww898): Skipped "Client", "Server" because the mention is already in 'ProductName'
          if (installationType != null && installationType != JetWindowsInstallationType.Client && installationType != JetWindowsInstallationType.Server)
            builder.Append(" (").Append(installationType).Append(')');

          builder.Append(' ').Append(buildNumber);
          if (ubr != null)
            builder.Append('.').Append(ubr.Value);
          return new OSInfo(builder.ToString(), buildNumber, installationType);
        });
    }

    private record struct RegQueryData(REG Type, byte[]? Data)
    {
      public uint AsDWord()
      {
        if (Type != REG.REG_DWORD)
          throw new FormatException("Expected REG_DWORD");
        if (Data == null)
          throw new NullReferenceException();
        return
          ((uint)Data[0] << 0) |
          ((uint)Data[1] << 8) |
          ((uint)Data[2] << 16) |
          ((uint)Data[3] << 24);
      }

      public string AsString()
      {
        if (Type != REG.REG_SZ)
          throw new FormatException("Expected REG_SZ");
        if (Data == null)
          throw new NullReferenceException();
        var chars = Encoding.Unicode.GetChars(Data);
        var n = 0;
        while (n < chars.Length && chars[n] != '\0')
          ++n;
        return new string(chars, 0, n);
      }

      public uint? AsOptionalDWord() => Type == REG.REG_DWORD ? AsDWord() : null;
      public string? AsOptionalString() => Type == REG.REG_SZ ? AsString() : null;
    }

    private delegate RegQueryData RegQueryValueDelegate(string valueName);
    private delegate TResult RegQueryValuesDelegate<out TResult>(RegQueryValueDelegate query);

    private static unsafe TResult? RegQueryValues<TResult>(void* hKey, string path, KeyAccessRights samDesired, RegQueryValuesDelegate<TResult> queries)
    {
      void* hSubKey;
      var error = Advapi32Dll.RegOpenKeyExW(hKey, path, 0, (uint)(KeyAccessRights.KEY_QUERY_VALUE | samDesired), &hSubKey);
      switch (error)
      {
      case WinError.ERROR_SUCCESS: break;
      case WinError.ERROR_FILE_NOT_FOUND: return default;
      default: throw new Win32Exception(error);
      }
      var hSubKey2 = hSubKey;

      try
      {
        return queries(valueName =>
          {
            while (true)
            {
              REG dwType;
              uint cbData;
              error = Advapi32Dll.RegQueryValueExW(hSubKey2, valueName, null, &dwType, null, &cbData);
              switch (error)
              {
              case WinError.ERROR_SUCCESS: break;
              case WinError.ERROR_FILE_NOT_FOUND: return new(REG.REG_NONE, null);
              default: throw new Win32Exception(error);
              }

              var data = new byte[cbData];
              if (cbData == 0)
                return new(dwType, data);
              fixed (byte* pdata = data)
                error = Advapi32Dll.RegQueryValueExW(hSubKey2, valueName, null, &dwType, pdata, &cbData);
              switch (error)
              {
              case WinError.ERROR_SUCCESS: return new(dwType, data);
              case WinError.ERROR_FILE_NOT_FOUND: return new(REG.REG_NONE, null);
              case WinError.ERROR_MORE_DATA: continue;
              default: throw new Win32Exception(error);
              }
            }
          });
      }
      finally
      {
        Advapi32Dll.RegCloseKey(hSubKey2);
      }
    }

    internal record struct ElevationData(bool IsUserAdministrator, bool IsElevated, JetWindowsTokenElevationType ElevationType);
  }
}