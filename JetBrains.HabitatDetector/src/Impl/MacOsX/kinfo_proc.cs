#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [StructLayout(LayoutKind.Explicit, Size = 296)]
  internal struct extern_proc
  {
    [FieldOffset(32)]
    internal int p_flag;
  }

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [StructLayout(LayoutKind.Explicit, Size = 76)]
  internal struct _ucred
  {
    [FieldOffset(4)]
    internal uint cr_uid;
  }

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [StructLayout(LayoutKind.Explicit, Size = 352)]
  internal struct eproc
  {
    [FieldOffset(120)]
    internal _ucred e_ucred;
  }

  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
  [StructLayout(LayoutKind.Explicit, Size = 648)]
  internal struct kinfo_proc
  {
    [FieldOffset(0)]
    internal extern_proc kp_proc;

    [FieldOffset(296)]
    internal eproc kp_eproc;
  }
}
#endif