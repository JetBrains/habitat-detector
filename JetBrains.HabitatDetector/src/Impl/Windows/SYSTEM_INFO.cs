#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [StructLayout(LayoutKind.Sequential)]
  internal struct SYSTEM_INFO
  {
    /// <seealso cref="PROCESSOR_ARCHITECTURE"/>
    internal PROCESSOR_ARCHITECTURE wProcessorArchitecture;

    internal UInt16 wReserved;
    internal UInt32 dwPageSize;
    internal IntPtr lpMinimumApplicationAddress;
    internal IntPtr lpMaximumApplicationAddress;
    internal UIntPtr dwActiveProcessorMask;
    internal UInt32 dwNumberOfProcessors;
    internal UInt32 dwProcessorType;
    internal UInt32 dwAllocationGranularity;
    internal UInt16 wProcessorLevel;
    internal UInt16 wProcessorRevision;
  }
}
#endif