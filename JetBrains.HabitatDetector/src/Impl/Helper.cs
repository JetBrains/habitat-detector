using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JetBrains.HabitatDetector.Impl.Linux;
using JetBrains.HabitatDetector.Impl.MacOsX;
using JetBrains.HabitatDetector.Impl.Unix;
using JetBrains.HabitatDetector.Impl.Windows;

namespace JetBrains.HabitatDetector.Impl
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static class Helper
  {
    internal static readonly JetPlatform Platform;
    internal static readonly JetArchitecture OSArchitecture;
    internal static readonly JetArchitecture ProcessArchitecture;
    internal static readonly string OSName;

    internal static readonly JetClrImplementation ClrImplementation;
    internal static readonly Version? MonoVersion;

    internal static readonly JetLinuxLibC? LinuxLibC;

    internal static readonly Version? MacOSVersion;

    internal static readonly uint? WindowsBuildNumber;
    internal static readonly JetWindowsInstallationType? WindowsInstallationType;
    internal static readonly bool? WindowsIsUserAdministrator;
    internal static readonly bool? WindowsIsElevated;
    internal static readonly JetWindowsTokenElevationType? WindowsElevationType;

    static Helper()
    {
#if NETSTANDARD1_0
#error No OS detection possible

#elif NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
      var isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
      var isNetCore = !System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
#else
      var isWindows = Environment.OSVersion.Platform switch
        {
          PlatformID.Unix => false,
          PlatformID.Win32NT => true,
          _ => throw new PlatformNotSupportedException($"Unsupported platform identifier {Environment.OSVersion.Platform}")
        };
      var isNetCore = typeof(object).Assembly.FullName.StartsWith("System.Private.CoreLib", StringComparison.Ordinal);
#endif

      if (isWindows)
      {
        Platform = JetPlatform.Windows;
        ProcessArchitecture = WindowsHelper.GetProcessArchitecture();
        OSArchitecture = WindowsHelper.GetOSArchitecture();
#pragma warning disable CS8624
        (OSName, WindowsBuildNumber, WindowsInstallationType) = WindowsHelper.GetOSInfo();
        (WindowsIsUserAdministrator, WindowsIsElevated, WindowsElevationType) = WindowsHelper.GetElevation();
#pragma warning restore CS8624
      }
      else
      {
        (Platform, var unameSysname, var unameRelease, var unameArchitecture) = UnixHelper.GetUnameInfo();
        switch (Platform)
        {
        case JetPlatform.FreeBSD:
          ProcessArchitecture = unameArchitecture;
          OSArchitecture = unameArchitecture;
          OSName = UnixHelper.GetOSName(Platform, unameSysname, unameRelease);
          break;
        case JetPlatform.Linux:
          // Note(ww898): Do not use `unameArchitecture` on Linux because 32-bit docker can be run on 64-bit host!!!
#pragma warning disable CS8624
          (LinuxLibC, ProcessArchitecture) = LinuxHelper.GetElfInfo();
#pragma warning restore CS8624
          OSArchitecture = ProcessArchitecture; // Note(ww898): Normally OsArchitecture == ProcessArchitecture on Linux!!!!
          OSName = UnixHelper.GetOSName(Platform, unameSysname, unameRelease);
          break;
        case JetPlatform.MacOsX:
          ProcessArchitecture = unameArchitecture;
          OSArchitecture = MacOsHelper.GetRunningUnderRosetta2() ? JetArchitecture.Arm64 : unameArchitecture; // Note(ww898): Process under Rosetta2 works only on ARM64 host!
          MacOSVersion = NormalizeVersion(MacOsHelper.GetOSVersion(unameArchitecture));
          OSName = MacOsHelper.GetOSName(MacOSVersion);
          break;
        default: throw new PlatformNotSupportedException($"Unsupported platform {Platform}");
        }
      }

      var monoRuntimeType = Type.GetType("Mono.Runtime");
      if (monoRuntimeType != null)
      {
#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
        var displayNameInfo = monoRuntimeType.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
        if (displayNameInfo != null)
        {
          var displayNameStr = (string)displayNameInfo.Invoke(null, null);
          var n = displayNameStr.IndexOf(' ');
          var versionStr = n >= 0 ? displayNameStr.Substring(0, n) : displayNameStr;

          Version? monoVersion;
#if NET20 || NET30 || NET35
          try
          {
            monoVersion = new Version(versionStr);
          }
          catch
          {
            monoVersion = null;
          }
#else
          Version.TryParse(versionStr, out monoVersion);
#endif

          if (monoVersion != null)
            MonoVersion = NormalizeVersion(monoVersion);
        }
#endif

        ClrImplementation = JetClrImplementation.Mono;
      }
      else
        ClrImplementation = isNetCore ? JetClrImplementation.NetCore : JetClrImplementation.NetFramework;
    }

    internal static Version NormalizeVersion(Version version) => version.Build > 0
      ? version.Revision == -1
        ? version
        : new(version.Major, version.Minor, version.Build)
      : new(version.Major, version.Minor);

    internal static unsafe JetArchitecture GetProcessArchitecture(int processId) => Platform switch
      {
        JetPlatform.FreeBSD or JetPlatform.Linux => OSArchitecture,
        JetPlatform.MacOsX => MacOsHelper.GetRunningUnderRosetta2(processId)
          ? JetArchitecture.X64
          : OSArchitecture,
        JetPlatform.Windows => WindowsHelper.OpenProcess(processId, WindowsHelper.GetProcessArchitecture),
        _ => throw new PlatformNotSupportedException()
      };

    internal static unsafe JetArchitecture GetProcessArchitecture(void* processHandle) => Platform switch
      {
        JetPlatform.Windows => WindowsHelper.GetProcessArchitecture(processHandle),
        _ => throw new PlatformNotSupportedException()
      };

#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
    internal static unsafe JetArchitecture GetProcessArchitecture(Process process) => Platform switch
      {
        JetPlatform.FreeBSD or JetPlatform.Linux => OSArchitecture,
        JetPlatform.MacOsX => MacOsHelper.GetRunningUnderRosetta2(process.Id)
          ? JetArchitecture.X64
          : OSArchitecture,
        JetPlatform.Windows => WindowsHelper.GetProcessArchitecture(process.Handle.ToPointer()),
        _ => throw new PlatformNotSupportedException()
      };
#endif
  }
}