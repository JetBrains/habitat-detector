using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static class LibObjC
  {
    private const string LibraryName = "/usr/lib/libobjc.dylib";

    [DllImport(LibraryName, ExactSpelling = true)]
    internal static extern unsafe void* objc_getClass([MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport(LibraryName, ExactSpelling = true)]
    internal static extern unsafe void* sel_getUid([MarshalAs(UnmanagedType.LPStr)] string str);

    [DllImport(LibraryName, ExactSpelling = true)]
    internal static extern unsafe void* objc_msgSend(void* self, void* op);

    [DllImport(LibraryName, ExactSpelling = true)]
    internal static extern unsafe void objc_msgSend_stret(out void* stret, void* self, void* op);

    [DllImport(LibraryName, ExactSpelling = true, EntryPoint = "objc_msgSend")]
    internal static extern unsafe NSOperatingSystemVersion NSOperatingSystemVersion_objc_msgSend(void* self, void* op);

    [DllImport(LibraryName, ExactSpelling = true, EntryPoint = "objc_msgSend_stret")]
    internal static extern unsafe void NSOperatingSystemVersion_objc_msgSend_stret(out NSOperatingSystemVersion osVersion, void* self, void* op);

    [StructLayout(LayoutKind.Sequential)]
    internal struct NSOperatingSystemVersion
    {
      public nint majorVersion;
      public nint minorVersion;
      public nint patchVersion;
    }
  }
}