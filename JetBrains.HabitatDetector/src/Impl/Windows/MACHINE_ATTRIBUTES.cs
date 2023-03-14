using System;
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [Flags]
  internal enum MACHINE_ATTRIBUTES
  {
    UserEnabled = 0x00000001,
    KernelEnabled = 0x00000002,
    Wow64Container = 0x00000004
  }
}