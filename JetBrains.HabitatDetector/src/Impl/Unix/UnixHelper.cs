using System;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Unix
{
  internal static class UnixHelper
  {
    internal static UnameInfo GetUnameInfo()
    {
      var buf = IntPtr.Zero;
      try
      {
        // from sys/utsname.h:
        // struct utsname
        // {
        //   char sysname[NAMELEN or SYS_NMLN];
        //   char nodename[NAMELEN or SYS_NMLN];
        //   char release[NAMELEN or SYS_NMLN];
        //   char version[NAMELEN or SYS_NMLN];
        //   char machine[NAMELEN or SYS_NMLN];
        // };

        buf = Marshal.AllocHGlobal(8192);
        var rc = LibC.uname(buf);
        if (rc != 0)
          throw new Exception("uname() from libc returned " + rc);

        var sysname = Marshal.PtrToStringAnsi(buf) ?? throw new NullReferenceException();
        var platform = ConvertToPlatform(sysname);
        var nameLen = platform switch
          {
            JetPlatform.FreeBSD => 32,
            JetPlatform.Linux => 65,
            JetPlatform.MacOsX => 256,
            _ => throw new PlatformNotSupportedException()
          };
        const int machineIndex = 4;
        var machine = Marshal.PtrToStringAnsi((nint)buf + machineIndex * nameLen) ?? throw new NullReferenceException();
        return new UnameInfo(platform, ConvertToArchitecture(platform, machine));
      }
      finally
      {
        if (buf != IntPtr.Zero)
          Marshal.FreeHGlobal(buf);
      }
    }

    private static JetPlatform ConvertToPlatform(string sysname) => sysname switch
      {
        "Darwin" => JetPlatform.MacOsX,
        "FreeBSD" => JetPlatform.FreeBSD,
        "Linux" => JetPlatform.Linux,
        _ => throw new PlatformNotSupportedException($"Unknown sysname {sysname}")
      };

    private static JetArchitecture ConvertToArchitecture(JetPlatform platform, string machine) => platform switch
      {
        JetPlatform.FreeBSD => machine switch
          {
            "amd64" => JetArchitecture.X64,
            "arm64" => JetArchitecture.Arm64,
            _ => throw new ArgumentOutOfRangeException(nameof(machine), machine, null)
          },
        JetPlatform.Linux => machine switch
          {
            "aarch64" => JetArchitecture.Arm64,
            "armv7l" or "armv8l" => JetArchitecture.Arm,
            "i686" => JetArchitecture.X86,
            "loongarch64" => JetArchitecture.LoongArch64,
            "s390x" => JetArchitecture.S390x,
            "x86_64" => JetArchitecture.X64,
            _ => throw new ArgumentOutOfRangeException(nameof(machine), machine, null)
          },
        JetPlatform.MacOsX => machine switch
          {
            "arm64" => JetArchitecture.Arm64,
            "x86_64" => JetArchitecture.X64,
            _ => throw new ArgumentOutOfRangeException(nameof(machine), machine, null)
          },
        _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
      };

    internal record struct UnameInfo(JetPlatform Platform, JetArchitecture KernelArchitecture);
  }
}