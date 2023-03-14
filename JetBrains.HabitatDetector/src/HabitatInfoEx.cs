#if !(NET20 || NET30)
namespace JetBrains.HabitatDetector
{
  public static class HabitatInfoEx
  {
    public static string ToPlatformIdName(this JetPlatform platform) => HabitatInfo.GetRuntimeIdName(platform);
    public static string ToPlatformIdName(this JetArchitecture architecture) => HabitatInfo.GetRuntimeIdName(architecture);
    public static string ToPlatformIdName(this JetLinuxLibC linuxLibC) => HabitatInfo.GetRuntimeIdName(linuxLibC);

    public static string ToPresentableString(this JetClrImplementation clrImplementation) => HabitatInfo.GetPresentableString(clrImplementation);
    public static string ToPresentableString(this JetPlatform platform) => HabitatInfo.GetPresentableString(platform);
    public static string ToPresentableString(this JetArchitecture architecture) => HabitatInfo.GetPresentableString(architecture);

#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
    public static JetArchitecture GetArchitecture(this System.Diagnostics.Process process) => HabitatInfo.GetProcessArchitecture(process);
#endif
  }
}
#endif