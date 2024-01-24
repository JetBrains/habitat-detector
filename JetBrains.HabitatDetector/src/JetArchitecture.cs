using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector
{
  /// <summary>
  ///   This is the copy <see cref="System.Runtime.InteropServices.Architecture" /> form
  ///   https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Runtime/InteropServices/Architecture.cs
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public enum JetArchitecture
  {
    X86 = 0,
    X64 = 1,
    Arm = 2,
    Arm64 = 3,
    S390x = 5,
    LoongArch64 = 6,
    Ppc64le = 8,
    RiscV64 = 9
  }
}