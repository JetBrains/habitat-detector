using System;
using System.Diagnostics;
using JetBrains.HabitatDetector.Impl.Linux;
using JetBrains.HabitatDetector.Impl.MacOsX;
using JetBrains.HabitatDetector.Impl.Unix;
using JetBrains.HabitatDetector.Impl.Windows;

namespace JetBrains.HabitatDetector.Impl
{
  internal static class Helper
  {
    internal static readonly JetClrImplementation ClrImplementation;
    internal static readonly JetPlatform Platform;
    internal static readonly JetArchitecture OSArchitecture;
    internal static readonly JetArchitecture ProcessArchitecture;
    internal static readonly JetLinuxLibC? LinuxLibC;
    internal static readonly JetWindowsInstallationType? WindowsInstallationType;
    internal static readonly bool? WindowsIsUserAdministrator;
    internal static readonly bool? WindowsIsElevated;
    internal static readonly JetWindowsTokenElevationType? WindowsElevationType;

    static Helper()
    {
#if NETSTANDARD1_0
#error No OS detection possible

#elif NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
      var isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
      var clrImplementation =
        System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase) ? JetClrImplementation.NetFramework :
        System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase) ? JetClrImplementation.NetCore :
        System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase) ? JetClrImplementation.NetCore :
        System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET", StringComparison.OrdinalIgnoreCase) ? JetClrImplementation.Net :
        throw new PlatformNotSupportedException("Unsupported CLR implementation");
#else
      var isWindows = Environment.OSVersion.Platform switch
        {
          PlatformID.Unix => false,
          PlatformID.Win32NT => true,
          _ => throw new PlatformNotSupportedException($"Unsupported platform identifier {Environment.OSVersion.Platform}")
        };
      var clrImplementation =
        Environment.Version.Major >= 5 ? JetClrImplementation.Net :
        typeof(object).Assembly.FullName.StartsWith("System.Private.CoreLib", StringComparison.Ordinal) ? JetClrImplementation.NetCore :
        JetClrImplementation.NetFramework;
#endif

      if (isWindows)
      {
        Platform = JetPlatform.Windows;
        ProcessArchitecture = WindowsHelper.GetProcessArchitecture();
        OSArchitecture = WindowsHelper.GetOSArchitecture();
        WindowsInstallationType = WindowsHelper.GetInstallationType();
#pragma warning disable CS8624
        (WindowsIsUserAdministrator, WindowsIsElevated, WindowsElevationType) = WindowsHelper.GetElevation();
#pragma warning restore CS8624
      }
      else
      {
        (Platform, var kernelArchitecture) = UnixHelper.GetUnameInfo();
        switch (Platform)
        {
        case JetPlatform.FreeBSD:
          ProcessArchitecture = kernelArchitecture;
          OSArchitecture = kernelArchitecture;
          break;
        case JetPlatform.Linux:
          // Note(ww898): Do not use `UnixHelper.KernelArchitecture` on Linux because 32-bit docker can be run on 64-bit host!!!
#pragma warning disable CS8624
          (LinuxLibC, ProcessArchitecture) = LinuxHelper.GetElfInfo();
#pragma warning restore CS8624
          OSArchitecture = ProcessArchitecture; // Note(ww898): Normally OsArchitecture == ProcessArchitecture on Linux!!!!
          break;
        case JetPlatform.MacOsX:
          ProcessArchitecture = kernelArchitecture;
          OSArchitecture = MacOsHelper.GetRunningUnderRosetta2() ? JetArchitecture.Arm64 : kernelArchitecture; // Note(ww898): Process under Rosetta2 works only on ARM64 host!
          break;
        default: throw new PlatformNotSupportedException($"Unsupported platform {Platform}");
        }
      }

      ClrImplementation = Type.GetType("Mono.Runtime") != null ? JetClrImplementation.Mono : clrImplementation;
    }

    internal static unsafe JetArchitecture GetProcessArchitecture(int processId) => Platform switch
      {
        JetPlatform.FreeBSD or JetPlatform.Linux => OSArchitecture,
        JetPlatform.MacOsX => MacOsHelper.GetRunningUnderRosetta2(processId)
          ? JetArchitecture.X64
          : OSArchitecture,
        JetPlatform.Windows => WindowsHelper.OpenProcess(processId, WindowsHelper.GetProcessArchitecture),
        _ => throw new PlatformNotSupportedException()
      };

    internal static unsafe JetArchitecture GetProcessArchitecture(void* processHandle) => Platform switch
      {
        JetPlatform.Windows => WindowsHelper.GetProcessArchitecture(processHandle),
        _ => throw new PlatformNotSupportedException()
      };

#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
    internal static unsafe JetArchitecture GetProcessArchitecture(Process process) => Platform switch
      {
        JetPlatform.FreeBSD or JetPlatform.Linux => OSArchitecture,
        JetPlatform.MacOsX => MacOsHelper.GetRunningUnderRosetta2(process.Id)
          ? JetArchitecture.X64
          : OSArchitecture,
        JetPlatform.Windows => WindowsHelper.GetProcessArchitecture(process.Handle.ToPointer()),
        _ => throw new PlatformNotSupportedException()
      };
#endif
  }
}