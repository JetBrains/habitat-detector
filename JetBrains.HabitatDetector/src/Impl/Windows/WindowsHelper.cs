using System;
using System.ComponentModel;
using System.Text;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  internal static class WindowsHelper
  {
    private static readonly Kernel32Dll.IsWow64ProcessDelegate? ourIsWow64Process;
    private static readonly Kernel32Dll.IsWow64Process2Delegate? ourIsWow64Process2;
    private static readonly Kernel32Dll.GetProcessInformationDelegate? ourGetProcessInformation;

    static unsafe WindowsHelper()
    {
      var hKernel32Dll = Kernel32Dll.GetModuleHandleW(Kernel32Dll.LibraryName);
      if (hKernel32Dll == null)
        throw new Win32Exception();

      var pIsWow64Process = Kernel32Dll.GetProcAddress(hKernel32Dll, nameof(Kernel32Dll.IsWow64Process));
      if (pIsWow64Process != null)
#pragma warning disable CS0618
        ourIsWow64Process = (Kernel32Dll.IsWow64ProcessDelegate)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)pIsWow64Process, typeof(Kernel32Dll.IsWow64ProcessDelegate));
#pragma warning restore CS0618

      var pIsWow64Process2 = Kernel32Dll.GetProcAddress(hKernel32Dll, nameof(Kernel32Dll.IsWow64Process2));
      if (pIsWow64Process2 != null)
#pragma warning disable CS0618
        ourIsWow64Process2 = (Kernel32Dll.IsWow64Process2Delegate)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)pIsWow64Process2, typeof(Kernel32Dll.IsWow64Process2Delegate));
#pragma warning restore CS0618

      var pGetProcessInformation = Kernel32Dll.GetProcAddress(hKernel32Dll, nameof(Kernel32Dll.GetProcessInformation));
      if (pGetProcessInformation != null)
#pragma warning disable CS0618
        ourGetProcessInformation = (Kernel32Dll.GetProcessInformationDelegate)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)pGetProcessInformation, typeof(Kernel32Dll.GetProcessInformationDelegate));
#pragma warning restore CS0618
    }

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
      if (ourGetProcessInformation != null)
      {
        PROCESS_MACHINE_INFORMATION pmi;
        if (ourGetProcessInformation(hProcess, PROCESS_INFORMATION_CLASS.ProcessMachineTypeInfo, &pmi, (uint)sizeof(PROCESS_MACHINE_INFORMATION)) == 0)
        {
          // Note(ww898): PROCESS_INFORMATION_CLASS.ProcessMachineTypeInfo can be not yet implemented. Available since Windows 10.0 Build 22000.
          var error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
          if (error != WinError.ERROR_INVALID_PARAMETER)
            throw new Win32Exception(error);
        }
        else
          return ConvertToArchitecture(pmi.ProcessMachine);
      }

      if (ourIsWow64Process2 != null)
      {
        // Bug(ww898): We can't detect X64 processes on ARM64 OS here!!!
        IMAGE_FILE_MACHINE processImageFileMachine, nativeImageFileMachine;
        if (ourIsWow64Process2(hProcess, &processImageFileMachine, &nativeImageFileMachine) == 0)
          throw new Win32Exception();
        return ConvertToArchitecture(processImageFileMachine == IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_UNKNOWN
          ? nativeImageFileMachine
          : processImageFileMachine);
      }

      if (ourIsWow64Process != null)
      {
        int isWow64;
        if (ourIsWow64Process(hProcess, &isWow64) == 0)
          throw new Win32Exception();
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
      if (ourIsWow64Process2 != null)
      {
        IMAGE_FILE_MACHINE processImageFileMachine, nativeImageFileMachine;
        if (ourIsWow64Process2(Kernel32Dll.GetCurrentProcess(), &processImageFileMachine, &nativeImageFileMachine) == 0)
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
          var ntAuthority = SID_IDENTIFIER_AUTHORITY.SECURITY_NT_AUTHORITY;
          void* administratorsGroupSid;
          if (Advapi32Dll.AllocateAndInitializeSid(
                &ntAuthority,
                2,
                RID.SECURITY_BUILTIN_DOMAIN_RID,
                RID.DOMAIN_ALIAS_RID_ADMINS,
                0, 0, 0, 0, 0, 0,
                &administratorsGroupSid) == 0)
            throw new Win32Exception();
          try
          {
            int isMember;
            if (Advapi32Dll.CheckTokenMembership(hImpersonationToken, administratorsGroupSid, &isMember) == 0)
              throw new Win32Exception();
            return new(isMember != 0, te.TokenIsElevated != 0, tet switch
              {
                TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault => JetWindowsTokenElevationType.Default,
                TOKEN_ELEVATION_TYPE.TokenElevationTypeFull => JetWindowsTokenElevationType.Full,
                TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited => JetWindowsTokenElevationType.Limited,
                _ => throw new ArgumentOutOfRangeException()
              });
          }
          finally
          {
            Advapi32Dll.FreeSid(administratorsGroupSid);
          }
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

    internal static unsafe JetWindowsInstallationType? GetInstallationType()
    {
      // Note(ww898): 32-bits processes should access 64-bits registry because correct data only in it! See https://learn.microsoft.com/en-us/windows/win32/sysinfo/registry-key-security-and-access-rights
      var (type, data) = GetRegValue(HKEY.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "InstallationType", KeyAccessRights.KEY_WOW64_64KEY);
      return type == REG.REG_SZ && data != null
        ? GetRegValueString(data) switch
          {
            "Client" => JetWindowsInstallationType.Client,
            "Nano Server" => JetWindowsInstallationType.NanoServer,
            "Server Core" => JetWindowsInstallationType.ServerCore,
            "Server" => JetWindowsInstallationType.Server,
            _ => null
          }
        : null;
    }

    private static string GetRegValueString(byte[] data)
    {
      var chars = Encoding.Unicode.GetChars(data);
      var n = 0;
      while (n < chars.Length && chars[n] != '\0')
        ++n;
      return new string(chars, 0, n);
    }

    private record struct RegQueryData(REG Type, byte[]? Data);

    private static unsafe RegQueryData GetRegValue(void* hKey, string path, string valueName, KeyAccessRights samDesired = 0)
    {
      void* hSubKey;
      var error = Advapi32Dll.RegOpenKeyExW(hKey, path, 0, (uint)(KeyAccessRights.KEY_QUERY_VALUE | samDesired), &hSubKey);
      switch (error)
      {
      case WinError.ERROR_SUCCESS: break;
      case WinError.ERROR_FILE_NOT_FOUND: return new(REG.REG_NONE, null);
      default: throw new Win32Exception(error);
      }

      try
      {
        while (true)
        {
          REG dwType;
          uint cbData;
          error = Advapi32Dll.RegQueryValueExW(hSubKey, valueName, null, &dwType, null, &cbData);
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
            error = Advapi32Dll.RegQueryValueExW(hSubKey, valueName, null, &dwType, pdata, &cbData);
          switch (error)
          {
          case WinError.ERROR_SUCCESS: return new(dwType, data);
          case WinError.ERROR_FILE_NOT_FOUND: return new(REG.REG_NONE, null);
          case WinError.ERROR_MORE_DATA: continue;
          default: throw new Win32Exception(error);
          }
        }
      }
      finally
      {
        Advapi32Dll.RegCloseKey(hSubKey);
      }
    }

    internal record struct ElevationData(bool IsUserAdministrator, bool IsElevated, JetWindowsTokenElevationType ElevationType);
  }
}