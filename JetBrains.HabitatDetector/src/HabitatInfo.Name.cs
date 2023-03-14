using System;
using System.Text;

namespace JetBrains.HabitatDetector
{
  public static partial class HabitatInfo
  {
    public static string GetRuntimeIdName(JetPlatform platform)
    {
      return platform switch
        {
          JetPlatform.Linux => "linux",
          JetPlatform.MacOsX => "macos",
          JetPlatform.Windows => "windows",
          _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
        };
    }

    public static string GetRuntimeIdName(JetArchitecture architecture)
    {
      return architecture switch
        {
          JetArchitecture.Arm => "arm",
          JetArchitecture.Arm64 => "arm64",
          JetArchitecture.LoongArch64 => "loongarch64",
          JetArchitecture.Ppc64le => "ppc64le",
          JetArchitecture.S390x => "s390x",
          JetArchitecture.X64 => "x64",
          JetArchitecture.X86 => "x86",
          _ => throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null)
        };
    }

    public static string GetRuntimeIdName(JetLinuxLibC linuxLibC, bool allowGlibc = true)
    {
      return linuxLibC switch
        {
          JetLinuxLibC.Bionic => "bionic",
          JetLinuxLibC.Glibc when allowGlibc => "glibc",
          JetLinuxLibC.Musl => "musl",
          _ => throw new ArgumentOutOfRangeException(nameof(linuxLibC), linuxLibC, null)
        };
    }

    public static string GetRuntimeIdString(JetPlatform platform, JetArchitecture architecture, JetLinuxLibC? linuxLibC)
    {
      if ((platform == JetPlatform.Linux && linuxLibC == null) ||
          (platform != JetPlatform.Linux && linuxLibC != null))
        throw new ArgumentOutOfRangeException(nameof(linuxLibC), linuxLibC, null);
      var builder = new StringBuilder();
      builder.Append(GetRuntimeIdName(platform));
      if (linuxLibC != null && linuxLibC.Value != JetLinuxLibC.Glibc)
        builder.Append('-').Append(GetRuntimeIdName(linuxLibC.Value, false));
      builder.Append('-').Append(GetRuntimeIdName(architecture));
      return builder.ToString();
    }
  }
}