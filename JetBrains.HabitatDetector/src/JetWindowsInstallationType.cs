using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public enum JetWindowsInstallationType
  {
    Client,
    NanoServer,
    Server,
    ServerCore
  }
}