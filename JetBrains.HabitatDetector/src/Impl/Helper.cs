using System;
using JetBrains.HabitatDetector.Impl.Linux;
using JetBrains.HabitatDetector.Impl.MacOsX;
using JetBrains.HabitatDetector.Impl.Unix;
using JetBrains.HabitatDetector.Impl.Windows;

namespace JetBrains.HabitatDetector.Impl
{
  internal static class Helper
  {
    internal static readonly JetPlatform Platform;
    internal static readonly JetArchitecture OSArchitecture;
    internal static readonly JetArchitecture ProcessArchitecture;
    internal static readonly JetLinuxLibC? LinuxLibC;
    internal static readonly JetLinuxDistro? LinuxDistro;
    internal static readonly JetWindowsInstallationType? WindowsInstallationType;

    static Helper()
    {
#if NETSTANDARD1_0
#error No OS detection possible

#elif NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
      var isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
#else
      var isWindows = Environment.OSVersion.Platform switch
        {
          PlatformID.Unix => false,
          PlatformID.Win32NT => true,
          _ => throw new PlatformNotSupportedException()
        };
#endif

      if (isWindows)
      {
        Platform = JetPlatform.Windows;
        ProcessArchitecture = WindowsHelper.GetProcessArchitecture();
        OSArchitecture = WindowsHelper.GetOSArchitecture();
        WindowsInstallationType = WindowsHelper.GetInstallationType();
      }
      else
      {
        (Platform, var kernelArchitecture) = UnixHelper.GetUnameInfo();
        switch (Platform)
        {
        case JetPlatform.Linux:
          // Note(ww898): Do not use `UnixHelper.KernelArchitecture` on Linux because 32-bit docker can be run on 64-bit host!!!
          (LinuxLibC, ProcessArchitecture) = LinuxHelper.GetElfInfo();
          OSArchitecture = ProcessArchitecture; // Note(ww898): Normally OsArchitecture == ProcessArchitecture on Linux!!!!
          LinuxDistro = LinuxHelper.GetOsReleaseInfo().LinuxDistro;
          break;
        case JetPlatform.MacOsX:
          ProcessArchitecture = kernelArchitecture;
          OSArchitecture = MacOsHelper.GetRunningUnderRosetta2() ? JetArchitecture.Arm64 : kernelArchitecture; // Note(ww898): Process under Rosetta2 works only on ARM64 host!
          break;
        default: throw new PlatformNotSupportedException();
        }
      }
    }
  }
}