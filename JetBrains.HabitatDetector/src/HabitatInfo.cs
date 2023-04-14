using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.HabitatDetector.Impl;

namespace JetBrains.HabitatDetector
{
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public static partial class HabitatInfo
  {
    public static JetClrImplementation ClrImplementation => Helper.ClrImplementation;
    public static Version? MonoVersion => Helper.MonoVersion;
    public static JetPlatform Platform => Helper.Platform;
    public static JetArchitecture OSArchitecture => Helper.OSArchitecture;
    public static JetArchitecture ProcessArchitecture => Helper.ProcessArchitecture;
    public static JetLinuxLibC? LinuxLibC => Helper.LinuxLibC;
    public static JetWindowsInstallationType? WindowsInstallationType => Helper.WindowsInstallationType;
    public static bool? WindowsIsUserAdministrator => Helper.WindowsIsUserAdministrator;
    public static bool? WindowsIsElevated => Helper.WindowsIsElevated;
    public static JetWindowsTokenElevationType? WindowsElevationType => Helper.WindowsElevationType;

    public static JetArchitecture[] AllOrderedArchitectures => GetAllOrderedArchitecturesForOS(Platform, OSArchitecture);

    public static JetArchitecture[] GetAllOrderedArchitecturesForOS(JetPlatform platform, JetArchitecture osArchitecture)
    {
      return platform == JetPlatform.Windows && osArchitecture == JetArchitecture.Arm64 ? new[] { JetArchitecture.Arm64, JetArchitecture.X64, JetArchitecture.X86 } :
        platform == JetPlatform.Windows && osArchitecture == JetArchitecture.X64 ? new[] { JetArchitecture.X64, JetArchitecture.X86 } :
        platform == JetPlatform.MacOsX && osArchitecture == JetArchitecture.Arm64 ? new[] { JetArchitecture.Arm64, JetArchitecture.X64 } :
        new[] { osArchitecture };
    }

    public static JetArchitecture GetProcessArchitecture(int processId) => Helper.GetProcessArchitecture(processId);
    public static unsafe JetArchitecture GetProcessArchitecture(void* processHandle) => Helper.GetProcessArchitecture(processHandle);

#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
    public static JetArchitecture GetProcessArchitecture(Process process) => Helper.GetProcessArchitecture(process);
#endif
  }
}