using System;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Unix
{
  internal static class LibC
  {
    private const string LibraryName = "libc"; // Note: No extension here, because CoreCLR support that case

    [DllImport(LibraryName, SetLastError = true)]
    internal static extern int uname(IntPtr buf);
  }
}