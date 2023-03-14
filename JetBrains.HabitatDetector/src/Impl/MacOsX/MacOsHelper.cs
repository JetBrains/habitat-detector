using System;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  internal static class MacOsHelper
  {
    internal static bool GetRunningUnderRosetta2() => GetRunningUnderRosetta2(LibSystemKernelDylib.getpid());
    internal static bool GetRunningUnderRosetta2(int processId) => GetSysctlKernProcPidTranslated(processId) ?? false;

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