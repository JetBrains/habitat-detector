using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static class WinError
  {
    internal const int ERROR_SUCCESS = 0;
    internal const int ERROR_FILE_NOT_FOUND = 2;
    internal const int ERROR_MORE_DATA = 234;
  }
}