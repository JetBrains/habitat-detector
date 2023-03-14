#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static class P_FLAG
  {
    internal const int P_TRANSLATED = 0x00020000;
  }
}
#endif