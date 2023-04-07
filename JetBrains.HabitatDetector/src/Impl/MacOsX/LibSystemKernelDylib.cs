using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static class LibSystemKernelDylib
  {
    private const string LibraryName = "/usr/lib/system/libsystem_kernel.dylib";

    [DllImport(LibraryName)]
    internal static extern int getpid();
  }
}