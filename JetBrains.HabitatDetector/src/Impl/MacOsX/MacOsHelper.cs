#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  internal static class MacOsHelper
  {
    internal static bool GetRunningUnderRosetta2() => GetSysctlKernProcPidTranslated(LibSystemKernelDylib.getpid()) ?? false;

    private static unsafe bool? GetSysctlKernProcPidTranslated(int pid)
    {
      const int mibSize = 4;
      var mib = stackalloc int[mibSize] { SYSCTL.CTL_KERN, SYSCTL.KERN_PROC, SYSCTL.KERN_PROC_PID, pid };
      var buf = new kinfo_proc();
      var bufLen = (ulong)sizeof(kinfo_proc);
      if (LibSystemCDylib.sysctl(mib, mibSize, &buf, &bufLen, null, 0) == -1)
      {
        if (Marshal.GetLastWin32Error() == ERRNO.ENOENT)
          return null;

        throw new Exception($"Failed to get the kinfo_proc structure with sysctl(CTL_KERN,KERN_PROC,KERN_PROC_PID,{pid})");
      }

      return (buf.kp_proc.p_flag & P_FLAG.P_TRANSLATED) != 0;
    }
  }
}
#endif