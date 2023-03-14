namespace JetBrains.HabitatDetector
{
  public enum JetClrImplementation
  {
    Mono,

    /// <summary>
    ///   .NET 5.0+
    /// </summary>
    Net,

    /// <summary>
    ///   .NET Core 1.0-3.1
    /// </summary>
    NetCore,

    NetFramework
  }
}