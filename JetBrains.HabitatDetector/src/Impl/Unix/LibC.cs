#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Unix
{
  internal static class LibC
  {
    private const string LibraryName = "libc"; // Note: No extension here, because CoreCLR support that case

    [DllImport(LibraryName, SetLastError = true)]
    internal static extern Int32 uname(IntPtr buf);
  }
}
#endif