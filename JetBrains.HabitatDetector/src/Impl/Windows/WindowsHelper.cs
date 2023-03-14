#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

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
          var isWow64Process2 = (Kernel32Dll.IsWow64Process2Delegate)Marshal.GetDelegateForFunctionPointer((IntPtr)pIsWow64Process2, typeof(Kernel32Dll.IsWow64Process2Delegate));
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
          var getNativeSystemInfo = (Kernel32Dll.GetNativeSystemInfoDelegate)Marshal.GetDelegateForFunctionPointer((IntPtr)pGetNativeSystemInfo, typeof(Kernel32Dll.GetNativeSystemInfoDelegate));
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
  }
}
#endif