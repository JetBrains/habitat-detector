using JetBrains.HabitatDetector.Impl;

namespace JetBrains.HabitatDetector
{
  public static partial class HabitatInfo
  {
    public static JetPlatform Platform => Helper.Platform;
    public static JetArchitecture OSArchitecture => Helper.OSArchitecture;
    public static JetArchitecture ProcessArchitecture => Helper.ProcessArchitecture;
    public static JetLinuxLibC? LinuxLibC => Helper.LinuxLibC;

    public static JetArchitecture[] AllOrderedArchitectures => GetAllOrderedArchitecturesForOS(Platform, OSArchitecture);

    public static JetArchitecture[] GetAllOrderedArchitecturesForOS(JetPlatform platform, JetArchitecture osArchitecture)
    {
      return platform == JetPlatform.Windows && osArchitecture == JetArchitecture.Arm64 ? new[] { JetArchitecture.Arm64, JetArchitecture.X64, JetArchitecture.X86 } :
        platform == JetPlatform.Windows && osArchitecture == JetArchitecture.X64 ? new[] { JetArchitecture.X64, JetArchitecture.X86 } :
        platform == JetPlatform.MacOsX && osArchitecture == JetArchitecture.Arm64 ? new[] { JetArchitecture.Arm64, JetArchitecture.X64 } :
        new[] { osArchitecture };
    }
  }
}