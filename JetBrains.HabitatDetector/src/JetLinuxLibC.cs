namespace JetBrains.HabitatDetector
{
  public enum JetLinuxLibC
  {
    /// <summary>
    ///   Android
    /// </summary>
    Bionic,

    /// <summary>
    ///   Ordinary Linuxes such as CentOS, Debian, Fedora, Ubuntu, ...
    /// </summary>
    Glibc,

    /// <summary>
    ///   Apline Linux
    /// </summary>
    Musl
  }
}