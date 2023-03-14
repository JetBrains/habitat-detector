#if NETSTANDARD1_0
#error No OS detection possible

#elif NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System;
using System.Runtime.InteropServices;
using JetBrains.HabitatDetector.Impl.Linux;

#else
using System;
using JetBrains.HabitatDetector.Impl.Linux;
using JetBrains.HabitatDetector.Impl.MacOsX;
using JetBrains.HabitatDetector.Impl.Unix;
using JetBrains.HabitatDetector.Impl.Windows;

#endif

namespace JetBrains.HabitatDetector.Impl
{
  internal static class Helper
  {
    internal static readonly JetPlatform Platform;
    internal static readonly JetArchitecture OSArchitecture;
    internal static readonly JetArchitecture ProcessArchitecture;
    internal static readonly JetLinuxLibC? LinuxLibC;

    static Helper()
    {
#if NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
      static JetArchitecture ConvertToArchitecture(Architecture architecture) => architecture switch
        {
          Architecture.X86 => JetArchitecture.X86,
          Architecture.X64 => JetArchitecture.X64,
          Architecture.Arm => JetArchitecture.Arm,
          Architecture.Arm64 => JetArchitecture.Arm64,
          _ => throw new PlatformNotSupportedException($"Unsupported architecture {architecture}")
        };

      Platform =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? JetPlatform.Windows :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? JetPlatform.MacOsX :
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? JetPlatform.Linux : throw new PlatformNotSupportedException();
      ProcessArchitecture = ConvertToArchitecture(RuntimeInformation.ProcessArchitecture);
      OSArchitecture = ConvertToArchitecture(RuntimeInformation.OSArchitecture); 

      if (Platform == JetPlatform.Linux)
          (LinuxLibC, _) = LinuxHelper.GetElfInfo();
#else
      switch (Environment.OSVersion.Platform)
      {
      case PlatformID.Unix:
        (Platform, var kernelArchitecture) = UnixHelper.GetUnameInfo();
        switch (Platform)
        {
        case JetPlatform.Linux:
          (LinuxLibC, ProcessArchitecture) = LinuxHelper.GetElfInfo(); // Note(ww898): Do not use `UnixHelper.KernelArchitecture` on Linux because 32-bit docker can be run on 64-bit host!!!
          OSArchitecture = ProcessArchitecture; // Note(ww898): Normally OsArchitecture == ProcessArchitecture on Linux!!!!
          break;
        case JetPlatform.MacOsX:
          ProcessArchitecture = kernelArchitecture;
          OSArchitecture = MacOsHelper.GetRunningUnderRosetta2() ? JetArchitecture.Arm64 : kernelArchitecture; // Note(ww898): Process under Rosetta2 works only on ARM64 host!
          break;
        default: throw new PlatformNotSupportedException();
        }

        break;
      case PlatformID.Win32NT:
        Platform = JetPlatform.Windows;
        ProcessArchitecture = WindowsHelper.GetProcessArchitecture();
        OSArchitecture = WindowsHelper.GetOSArchitecture();
        break;
      default: throw new PlatformNotSupportedException();
      }
#endif
    }
  }
}