using System;
using System.Text;

namespace JetBrains.HabitatDetector
{
  public static partial class HabitatInfo
  {
    public static string OSRuntimeIdString => GetRuntimeIdString(Platform, OSArchitecture, LinuxLibC);
    public static string ProcessRuntimeIdString => GetRuntimeIdString(Platform, ProcessArchitecture, LinuxLibC);

    public static string GetRuntimeIdName(JetPlatform platform)
    {
      return platform switch
        {
          JetPlatform.FreeBSD => "freebsd",
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

    public static string GetPresentableString(JetClrImplementation clrImplementation)
    {
      return clrImplementation switch
        {
          JetClrImplementation.Mono => "Mono",
          JetClrImplementation.NetCore => ".NET Core",
          JetClrImplementation.NetFramework => ".NET Framework",
          _ => throw new ArgumentOutOfRangeException(nameof(clrImplementation), clrImplementation, null)
        };
    }

    public static string GetPresentableString(JetPlatform platform)
    {
      return platform switch
        {
          JetPlatform.FreeBSD => "FreeBSD",
          JetPlatform.Linux => "Linux",
          JetPlatform.MacOsX => "macOS",
          JetPlatform.Windows => "Windows",
          _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
        };
    }

    public static string GetPresentableString(JetArchitecture architecture)
    {
      return architecture switch
        {
          JetArchitecture.X86 => "x86", // x86, IA-32 and Intel
          JetArchitecture.X64 => "x64", // x64, x86_64, AMD64, and Intel 64
          JetArchitecture.Arm => "ARM", // ARM, Advanced RISC Machine, Acorn RISC Machine
          JetArchitecture.Arm64 => "ARM64", // ARM64, Aarch64, Advanced RISC Machine 64-bit
          JetArchitecture.S390x => "S/390x", // S/390x, s390x, z/Architecture
          JetArchitecture.LoongArch64 => "LA64", // LA64, LoongArch64, LoongArch 64-bit
          JetArchitecture.Ppc64le => "PPC64LE", // ppc64le, PowerPC 64-bit little-endian
          _ => throw new ArgumentOutOfRangeException(nameof(architecture), architecture, null)
        };
    }
  }
}