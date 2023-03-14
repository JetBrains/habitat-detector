#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static class LibSystemCDylib
  {
    private const string LibraryName = "/usr/lib/system/libsystem_c.dylib";

    [DllImport(LibraryName, SetLastError = true)]
    internal static extern unsafe int sysctl(int* name, uint namelen, void* oldp, ulong* oldlenp, void* newp, ulong newlen);
  }
}
#endif