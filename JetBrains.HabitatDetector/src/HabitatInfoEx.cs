#if !(NET20 || NET30)
namespace JetBrains.HabitatDetector
{
  public static class HabitatInfoEx
  {
    public static string ToPlatformIdName(this JetPlatform platform) => HabitatInfo.GetRuntimeIdName(platform);
    public static string ToPlatformIdName(this JetArchitecture architecture) => HabitatInfo.GetRuntimeIdName(architecture);
    public static string ToPlatformIdName(this JetLinuxLibC linuxLibC) => HabitatInfo.GetRuntimeIdName(linuxLibC);
  }
}
#endif