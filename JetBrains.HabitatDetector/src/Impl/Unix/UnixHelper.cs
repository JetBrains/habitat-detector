#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System;
using System.Diagnostics.CodeAnalysis;
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
        //   char sysname[NAMELEN];
        //   char nodename[NAMELEN];
        //   char release[NAMELEN];
        //   char version[NAMELEN];
        //   char machine[NAMELEN];
        // };

        buf = Marshal.AllocHGlobal(8192);
        var rc = LibC.uname(buf);
        if (rc != 0)
          throw new Exception("uname() from libc returned " + rc);

        var platform = ConvertToPlatform(Marshal.PtrToStringAnsi(buf) ?? throw new NullReferenceException());
        var nameLen = platform switch
          {
            JetPlatform.Linux => 65,
            JetPlatform.MacOsX => 256,
            _ => throw new PlatformNotSupportedException()
          };
        const int machineIndex = 4;
        return new UnameInfo(platform, ConvertToArchitecture(platform, Marshal.PtrToStringAnsi((nint)buf + machineIndex * nameLen) ?? throw new NullReferenceException()));
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
        "Linux" => JetPlatform.Linux,
        _ => throw new PlatformNotSupportedException()
      };

    private static JetArchitecture ConvertToArchitecture(JetPlatform platform, string machine) => platform switch
      {
        JetPlatform.Linux => machine switch
          {
            "aarch64" => JetArchitecture.Arm64,
            "armv7l" or "armv8l" => JetArchitecture.Arm,
            "i686" => JetArchitecture.X86,
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

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    internal readonly struct UnameInfo
    {
      private readonly JetPlatform myPlatform;
      private readonly JetArchitecture myKernelArchitecture;

      internal UnameInfo(JetPlatform platform, JetArchitecture kernelArchitecture)
      {
        myPlatform = platform;
        myKernelArchitecture = kernelArchitecture;
      }

      internal void Deconstruct(out JetPlatform platform, out JetArchitecture kernelArchitecture)
      {
        platform = myPlatform;
        kernelArchitecture = myKernelArchitecture;
      }
    }
  }
}
#endif