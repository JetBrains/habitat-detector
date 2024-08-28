using System;
using System.Runtime.InteropServices;
using System.Text;

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

    private static unsafe string? GetSysctlKernOsVersion()
    {
      const int mibSize = 2;
      var mib = stackalloc int[mibSize] { SYSCTL.CTL_KERN, SYSCTL.KERN_OSVERSION };
      ulong bufLen = 0;
      if (LibSystemCDylib.sysctl(mib, mibSize, null, &bufLen, null, 0) == -1)
      {
        if (Marshal.GetLastWin32Error() == ERRNO.ENOENT)
          return null;
        throw new Exception("Failed to get size for the build number string with sysctl(CTL_KERN,KERN_OSVERSION)");
      }

      var buf = new byte[bufLen];
      fixed (byte* bufPtr = buf)
        if (LibSystemCDylib.sysctl(mib, mibSize, bufPtr, &bufLen, null, 0) == -1)
          throw new Exception("Failed to get the build number string with sysctl(CTL_KERN,KERN_OSVERSION)");

      var chars = Encoding.UTF8.GetChars(buf);
      var charsZeroIndex = Array.IndexOf(chars, '\0');
      var charsLen = charsZeroIndex < 0 ? chars.Length : charsZeroIndex;
      return new string(chars, 0, charsLen);
    }

    internal static unsafe Version GetOSVersion(JetArchitecture processArchitecture)
    {
      var processInfo = LibObjC.objc_msgSend(LibObjC.objc_getClass("NSProcessInfo"), LibObjC.sel_getUid("processInfo"));
      var operationSystemVersionUid = LibObjC.sel_getUid("operatingSystemVersion");
      LibObjC.NSOperatingSystemVersion res;
      switch (processArchitecture)
      {
      case JetArchitecture.Arm64:
        res = LibObjC.NSOperatingSystemVersion_objc_msgSend(processInfo, operationSystemVersionUid);
        break;
      case JetArchitecture.X64:
        LibObjC.NSOperatingSystemVersion_objc_msgSend_stret(out res, processInfo, operationSystemVersionUid);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(processArchitecture), processArchitecture, null);
      }

      int major;
      int minor;
      int patch;
      checked
      {
        major = (int)res.majorVersion;
        minor = (int)res.minorVersion;
        patch = (int)res.patchVersion;
      }

      // Note(ww898): Compatibility with older software! See also SYSTEM_VERSION_COMPAT=1. Big Sur and newer OSes always returns 10.16!!!
      if (major == 10 && minor == 16)
      {
        major = 11;
        minor = 0;
      }

      return new Version(major, minor, patch);
    }

    internal static string GetOSName(Version version)
    {
      var build = GetSysctlKernOsVersion();

      var builder = new StringBuilder(version.Major switch
        {
          15 => "macOS Sequoia",
          14 => "macOS Sonoma",
          13 => "macOS Ventura",
          12 => "macOS Monterey",
          11 => "macOS Big Sur",
          10 => version.Minor switch
            {
              15 => "macOS Catalina",
              14 => "macOS Mojave",
              13 => "macOS High Sierra",
              12 => "macOS Sierra",
              11 => "OS X El Capitan",
              10 => "OS X Yosemite",
              9 => "OS X Mavericks",
              8 => "OS X Mountain Lion",
              7 => "Mac OS X Lion",
              6 => "Mac OS X Snow Leopard",
              5 => "Mac OS X Leopard",
              4 => "Mac OS X Tiger",
              3 => "Mac OS X Panther",
              2 => "Mac OS X Jaguar",
              1 => "Mac OS X Puma",
              0 => "Mac OS X Cheetah",
              _ => throw new InvalidOperationException("Invalid macOS 10 minor version")
            },
          0 or 1 or 2 or 3 or 4 or 5 or 6 or 7 or 8 or 9 => throw new InvalidOperationException("Invalid macOS major version"),
          _ => "macOS" // Note(ww898): The fallback for unknown macOS name!!!
        });

      builder.Append(' ').Append(version.Major).Append('.').Append(version.Minor);
      if (version.Build > 0)
        builder.Append('.').Append(version.Build);
      if (build != null)
        builder.Append(' ').Append(build);
      return builder.ToString();
    }
  }
}