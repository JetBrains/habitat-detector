namespace JetBrains.HabitatDetector
{
  public static partial class HabitatInfo
  {
    public static bool TryParsePlatform(string platformStr, out JetPlatform platform)
    {
      switch (platformStr)
      {
      case "linux":
        platform = JetPlatform.Linux;
        return true;
      case "macos":
        platform = JetPlatform.MacOsX;
        return true;
      case "windows":
        platform = JetPlatform.Windows;
        return true;
      default:
        platform = default;
        return false;
      }
    }

    public static bool TryParseArchitecture(string architectureStr, out JetArchitecture architecture)
    {
      switch (architectureStr)
      {
      case "arm":
        architecture = JetArchitecture.Arm;
        return true;
      case "arm64":
        architecture = JetArchitecture.Arm64;
        return true;
      case "loongarch64":
        architecture = JetArchitecture.LoongArch64;
        return true;
      case "ppc64le":
        architecture = JetArchitecture.Ppc64le;
        return true;
      case "s390x":
        architecture = JetArchitecture.S390x;
        return true;
      case "x64":
        architecture = JetArchitecture.X64;
        return true;
      case "x86":
        architecture = JetArchitecture.X86;
        return true;
      default:
        architecture = default;
        return false;
      }
    }

    public static bool TryParseLinuxLibC(string linuxLibCStr, out JetLinuxLibC linuxLibC, bool allowGlibc = true)
    {
      switch (linuxLibCStr)
      {
      case "bionic":
        linuxLibC = JetLinuxLibC.Bionic;
        return true;
      case "glibc" when allowGlibc:
        linuxLibC = JetLinuxLibC.Glibc;
        return true;
      case "musl":
        linuxLibC = JetLinuxLibC.Musl;
        return true;
      default:
        linuxLibC = default;
        return false;
      }
    }

    public static bool TryParseRuntimeId(string str, out JetPlatform platform, out JetArchitecture architecture, out JetLinuxLibC? linuxLibC)
    {
      var parts = str.Split('-');
      switch (parts.Length)
      {
      case 2:
        if (TryParsePlatform(parts[0], out platform) &&
            TryParseArchitecture(parts[1], out architecture))
        {
          linuxLibC = platform == JetPlatform.Linux ? JetLinuxLibC.Glibc : null;
          return true;
        }

        break;
      case 3:
        if (TryParsePlatform(parts[0], out platform) &&
            platform == JetPlatform.Linux &&
            TryParseLinuxLibC(parts[1], out var linuxLibC1, false) &&
            TryParseArchitecture(parts[2], out architecture))
        {
          linuxLibC = linuxLibC1;
          return true;
        }

        break;
      }

      platform = default;
      architecture = default;
      linuxLibC = null;
      return false;
    }
  }
}