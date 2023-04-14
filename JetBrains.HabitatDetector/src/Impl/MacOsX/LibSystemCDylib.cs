using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static class LibSystemCDylib
  {
    private const string LibraryName = "/usr/lib/system/libsystem_c.dylib";

    [DllImport(LibraryName, SetLastError = true, ExactSpelling = true)]
    internal static extern unsafe int sysctl(int* name, uint namelen, void* oldp, ulong* oldlenp, void* newp, ulong newlen);
  }
}