using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace JetBrains.HabitatDetector.Impl.Unix
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static class UnixHelper
  {
    internal record struct UnameInfo(JetPlatform Platform, string Sysname, string Release, JetArchitecture Architecture);

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
        const int releaseIndex = 2;
        const int machineIndex = 4;

        buf = Marshal.AllocHGlobal(8192);
        var rc = LibC.uname(buf);
        if (rc != 0)
          throw new Exception("uname() from LibC returned " + rc);

        var sysname = Marshal.PtrToStringAnsi(buf) ?? throw new NullReferenceException();
        var platform = ConvertToPlatform(sysname);
        var nameLen = ConvertToNameLength(platform);
        var release = Marshal.PtrToStringAnsi((nint)buf + releaseIndex * nameLen) ?? throw new NullReferenceException();
        var machine = Marshal.PtrToStringAnsi((nint)buf + machineIndex * nameLen) ?? throw new NullReferenceException();
        return new UnameInfo(platform, sysname, release, ConvertToArchitecture(platform, machine));
      }
      finally
      {
        if (buf != IntPtr.Zero)
          Marshal.FreeHGlobal(buf);
      }
    }

    internal static JetPlatform ConvertToPlatform(string sysname) => sysname switch
      {
        "Darwin" => JetPlatform.MacOsX,
        "FreeBSD" => JetPlatform.FreeBSD,
        "Linux" => JetPlatform.Linux,
        _ => throw new ArgumentOutOfRangeException(nameof(sysname), sysname, null)
      };

    internal static int ConvertToNameLength(JetPlatform platform) => platform switch
      {
        JetPlatform.FreeBSD => 32,
        JetPlatform.Linux => 65,
        JetPlatform.MacOsX => 256,
        _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
      };

    internal static JetArchitecture ConvertToArchitecture(JetPlatform platform, string machine) => platform switch
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
            "ppc64le" => JetArchitecture.Ppc64le,
            "riscv64" => JetArchitecture.RiscV64,
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

    internal static string GetOSName(JetPlatform platform, string unameSysname, string unameRelease)
    {
      var properties = OsReleaseProperties.ReadFromDefaultLocations();
      var builder = new StringBuilder();

      {
        var prettyName = properties?.TryGetValue(OsReleaseProperties.PrettyNameKey);
        if (prettyName != null)
          builder.Append(prettyName);
        else
        {
          var name = properties?.TryGetValue(OsReleaseProperties.NameKey);
          builder.Append(name ?? unameSysname);

          var version = properties?.TryGetValue(OsReleaseProperties.VersionKey);
          if (version != null)
            builder.Append(' ').Append(version);
        }
      }

      if (platform == JetPlatform.Linux)
        builder.Append(' ').Append(unameRelease);

      return builder.ToString();
    }
  }
}