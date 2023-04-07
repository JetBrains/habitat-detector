using System;
using System.ComponentModel;
using System.Text;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  internal static class WindowsHelper
  {
    internal static unsafe JetArchitecture GetProcessArchitecture()
    {
      SYSTEM_INFO systemInfo;
      Kernel32Dll.GetSystemInfo(&systemInfo);
      return ConvertToArchitecture(systemInfo.wProcessorArchitecture);
    }

    internal static unsafe JetArchitecture GetOSArchitecture()
    {
      // Note(ww898): GetNativeSystemInfo() on Windows ARM64 returns 9(PROCESSOR_ARCHITECTURE_AMD64) instead of 12(PROCESSOR_ARCHITECTURE_ARM64) for X64 processes!!! So, I should use Kernel32Dll.IsWow64Process2() to detect real OS architecture.
      var hModule = Kernel32Dll.GetModuleHandleW(Kernel32Dll.LibraryName);
      if (hModule != null)
      {
        var pIsWow64Process2 = Kernel32Dll.GetProcAddress(hModule, nameof(Kernel32Dll.IsWow64Process2));
        if (pIsWow64Process2 != null)
        {
#pragma warning disable CS0618
          var isWow64Process2 = (Kernel32Dll.IsWow64Process2Delegate)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)pIsWow64Process2, typeof(Kernel32Dll.IsWow64Process2Delegate));
#pragma warning restore CS0618

          IMAGE_FILE_MACHINE processImageFileMachine, nativeImageFileMachine;
          if (isWow64Process2(Kernel32Dll.GetCurrentProcess(), &processImageFileMachine, &nativeImageFileMachine) == 0)
            throw new Win32Exception();
          return ConvertToArchitecture(nativeImageFileMachine);
        }

        var pGetNativeSystemInfo = Kernel32Dll.GetProcAddress(hModule, nameof(Kernel32Dll.GetNativeSystemInfo));
        if (pGetNativeSystemInfo != null)
        {
#pragma warning disable CS0618
          var getNativeSystemInfo = (Kernel32Dll.GetNativeSystemInfoDelegate)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)pGetNativeSystemInfo, typeof(Kernel32Dll.GetNativeSystemInfoDelegate));
#pragma warning restore CS0618

          SYSTEM_INFO nativeSystemInfo;
          getNativeSystemInfo(&nativeSystemInfo);
          return ConvertToArchitecture(nativeSystemInfo.wProcessorArchitecture);
        }
      }

      SYSTEM_INFO systemInfo;
      Kernel32Dll.GetSystemInfo(&systemInfo);
      return ConvertToArchitecture(systemInfo.wProcessorArchitecture);
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
  }
}