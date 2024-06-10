using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class HabitatInfoTest
  {
    [TestCase(JetPlatform.FreeBSD, "freebsd")]
    [TestCase(JetPlatform.Linux, "linux")]
    [TestCase(JetPlatform.MacOsX, "macos")]
    [TestCase(JetPlatform.Windows, "windows")]
    [Test]
    public void PlatformTest(JetPlatform platform, string expectedPlatform)
    {
      Assert.AreEqual(expectedPlatform, HabitatInfo.GetRuntimeIdName(platform));
      Assert.IsTrue(HabitatInfo.TryParsePlatform(expectedPlatform, out var platform1));
      Assert.AreEqual(platform, platform1);
    }

    [TestCase(JetArchitecture.Arm, "arm")]
    [TestCase(JetArchitecture.Arm64, "arm64")]
    [TestCase(JetArchitecture.LoongArch64, "loongarch64")]
    [TestCase(JetArchitecture.Ppc64le, "ppc64le")]
    [TestCase(JetArchitecture.RiscV64, "riscv64")]
    [TestCase(JetArchitecture.S390x, "s390x")]
    [TestCase(JetArchitecture.X64, "x64")]
    [TestCase(JetArchitecture.X86, "x86")]
    [Test]
    public void ArchitectureTest(JetArchitecture architecture, string expectedArchitecture)
    {
      Assert.AreEqual(expectedArchitecture, HabitatInfo.GetRuntimeIdName(architecture));
      Assert.IsTrue(HabitatInfo.TryParseArchitecture(expectedArchitecture, out var architecture1));
      Assert.AreEqual(architecture, architecture1);
    }

    [TestCase(JetLinuxLibC.Bionic, "bionic")]
    [TestCase(JetLinuxLibC.Glibc, "glibc")]
    [TestCase(JetLinuxLibC.Musl, "musl")]
    [Test]
    public void LinuxLibCTest(JetLinuxLibC linuxLibC, string expectedLinuxLibC)
    {
      Assert.AreEqual(expectedLinuxLibC, HabitatInfo.GetRuntimeIdName(linuxLibC));
      Assert.IsTrue(HabitatInfo.TryParseLinuxLibC(expectedLinuxLibC, out var linuxLibC1));
      Assert.AreEqual(linuxLibC, linuxLibC1);
    }

    [Test]
    public void GlibcLinuxLibCTest()
    {
      Assert.That((TestDelegate)(() => HabitatInfo.GetRuntimeIdName(JetLinuxLibC.Glibc, false)), Throws.Exception);
      Assert.IsFalse(HabitatInfo.TryParseLinuxLibC("glibc", out _, false));
    }

    [TestCase(JetPlatform.FreeBSD, JetArchitecture.Arm64, null, "freebsd-arm64")]
    [TestCase(JetPlatform.FreeBSD, JetArchitecture.X64, null, "freebsd-x64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm, JetLinuxLibC.Glibc, "linux-arm")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm, JetLinuxLibC.Musl, "linux-musl-arm")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm64, JetLinuxLibC.Glibc, "linux-arm64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm64, JetLinuxLibC.Musl, "linux-musl-arm64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.LoongArch64, JetLinuxLibC.Glibc, "linux-loongarch64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Ppc64le, JetLinuxLibC.Glibc, "linux-ppc64le")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Ppc64le, JetLinuxLibC.Musl, "linux-musl-ppc64le")]
    [TestCase(JetPlatform.Linux, JetArchitecture.RiscV64, JetLinuxLibC.Glibc, "linux-riscv64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.RiscV64, JetLinuxLibC.Musl, "linux-musl-riscv64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.S390x, JetLinuxLibC.Glibc, "linux-s390x")]
    [TestCase(JetPlatform.Linux, JetArchitecture.S390x, JetLinuxLibC.Musl, "linux-musl-s390x")]
    [TestCase(JetPlatform.Linux, JetArchitecture.X64, JetLinuxLibC.Glibc, "linux-x64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.X64, JetLinuxLibC.Musl, "linux-musl-x64")]
    [TestCase(JetPlatform.MacOsX, JetArchitecture.Arm64, null, "macos-arm64")]
    [TestCase(JetPlatform.MacOsX, JetArchitecture.X64, null, "macos-x64")]
    [TestCase(JetPlatform.Windows, JetArchitecture.Arm, null, "windows-arm")]
    [TestCase(JetPlatform.Windows, JetArchitecture.Arm64, null, "windows-arm64")]
    [TestCase(JetPlatform.Windows, JetArchitecture.X64, null, "windows-x64")]
    [TestCase(JetPlatform.Windows, JetArchitecture.X86, null, "windows-x86")]
    [Test]
    public void RuntimeIdTest(JetPlatform platform, JetArchitecture architecture, JetLinuxLibC? linuxLibC, string expectedPlatformIdStr)
    {
      Assert.AreEqual(expectedPlatformIdStr, HabitatInfo.GetRuntimeIdString(platform, architecture, linuxLibC));
      Assert.IsTrue(HabitatInfo.TryParseRuntimeId(expectedPlatformIdStr, out var platform1, out var architecture1, out var linuxLibC1));
      Assert.AreEqual(platform, platform1);
      Assert.AreEqual(architecture, architecture1);
      Assert.AreEqual(linuxLibC, linuxLibC1);
    }

    [TestCase("")]
    [TestCase("-")]
    [TestCase("--")]
    [TestCase("xxx")]
    [TestCase("xxx-x64")]
    [TestCase("xxx-musl-x64")]
    [TestCase("x64")]
    [TestCase("musl")]
    [TestCase("windows")]
    [TestCase("linux")]
    [TestCase("macos")]
    [TestCase("macos-x64-")]
    [TestCase("-macos-x64")]
    [TestCase("windows--x64")]
    [TestCase("windows-xxx")]
    [TestCase("windows-musl-x64")]
    [TestCase("linux-glibc-x64")]
    [TestCase("linux-musl-x64-")]
    [TestCase("-linux-musl-x64")]
    [TestCase("linux-musl--x64")]
    [TestCase("linux--musl-x64")]
    [TestCase("linux-musl-x64-xxx")]
    [Test]
    public void RuntimeIdErrorTest(string platformIdStr) => Assert.False(HabitatInfo.TryParseRuntimeId(platformIdStr, out _, out _, out _));

    [Test]
    public void OrderedArchitecturesTest()
    {
      static void Check(JetPlatform platform, JetArchitecture osArchitecture, params JetArchitecture[] expectedOrderedArchitectures)
      {
        var orderedArchitectures = HabitatInfo.GetAllOrderedArchitecturesForOS(platform, osArchitecture);
        Assert.AreEqual(expectedOrderedArchitectures.Length, orderedArchitectures.Length);
        for (var n = 0; n < expectedOrderedArchitectures.Length; n++)
          Assert.AreEqual(expectedOrderedArchitectures[n], orderedArchitectures[n]);
      }

      Check(JetPlatform.Linux, JetArchitecture.Arm, JetArchitecture.Arm);
      Check(JetPlatform.Linux, JetArchitecture.Arm64, JetArchitecture.Arm64);
      Check(JetPlatform.Linux, JetArchitecture.LoongArch64, JetArchitecture.LoongArch64);
      Check(JetPlatform.Linux, JetArchitecture.Ppc64le, JetArchitecture.Ppc64le);
      Check(JetPlatform.Linux, JetArchitecture.RiscV64, JetArchitecture.RiscV64);
      Check(JetPlatform.Linux, JetArchitecture.S390x, JetArchitecture.S390x);
      Check(JetPlatform.Linux, JetArchitecture.X64, JetArchitecture.X64);
      Check(JetPlatform.Linux, JetArchitecture.X86, JetArchitecture.X86);

      Check(JetPlatform.MacOsX, JetArchitecture.Arm64, JetArchitecture.Arm64, JetArchitecture.X64);
      Check(JetPlatform.MacOsX, JetArchitecture.X64, JetArchitecture.X64);

      Check(JetPlatform.Windows, JetArchitecture.Arm64, JetArchitecture.Arm64, JetArchitecture.X64, JetArchitecture.X86);
      Check(JetPlatform.Windows, JetArchitecture.X64, JetArchitecture.X64, JetArchitecture.X86);
      Check(JetPlatform.Windows, JetArchitecture.X86, JetArchitecture.X86);
    }

    [TestCase(JetClrImplementation.Mono, "Mono")]
    [TestCase(JetClrImplementation.NetCore, ".NET Core")]
    [TestCase(JetClrImplementation.NetFramework, ".NET Framework")]
    [Test]
    public void ClrImplementationPresentableTest(JetClrImplementation clrImplementation, string expectedName)
    {
      Assert.AreEqual(expectedName, HabitatInfo.GetPresentableString(clrImplementation));
    }

    [TestCase(JetPlatform.FreeBSD, "FreeBSD")]
    [TestCase(JetPlatform.Linux, "Linux")]
    [TestCase(JetPlatform.MacOsX, "macOS")]
    [TestCase(JetPlatform.Windows, "Windows")]
    [Test]
    public void PlatformPresentableTest(JetPlatform platform, string expectedName)
    {
      Assert.AreEqual(expectedName, HabitatInfo.GetPresentableString(platform));
    }

    [TestCase(JetArchitecture.Arm, "ARM")]
    [TestCase(JetArchitecture.Arm64, "ARM64")]
    [TestCase(JetArchitecture.LoongArch64, "LA64")]
    [TestCase(JetArchitecture.Ppc64le, "PPC64LE")]
    [TestCase(JetArchitecture.RiscV64, "RISC-V64")]
    [TestCase(JetArchitecture.S390x, "S/390x")]
    [TestCase(JetArchitecture.X64, "x64")]
    [TestCase(JetArchitecture.X86, "x86")]
    [Test]
    public void ArchitecturePresentableTest(JetArchitecture architecture, string expectedName)
    {
      Assert.IsTrue(expectedName.All(ch => ch is
        '/' or
        '-' or
        >= 'a' and <= 'z' or
        >= 'A' and <= 'Z' or
        >= '0' and <= '9'));
      Assert.AreEqual(expectedName, HabitatInfo.GetPresentableString(architecture));
    }

    [Test]
    public void CurrentTest()
    {
      Console.WriteLine(nameof(Environment) + "." + nameof(Environment.OSVersion)                                                   + ": {0}", Environment.OSVersion            );
      Console.WriteLine(nameof(Environment) + "." + nameof(Environment.OSVersion) + "." + nameof(Environment.OSVersion.Platform)    + ": {0}", Environment.OSVersion.Platform   );
      Console.WriteLine(nameof(Environment) + "." + nameof(Environment.OSVersion) + "." + nameof(Environment.OSVersion.Version)     + ": {0}", Environment.OSVersion.Version    );
      Console.WriteLine(nameof(Environment) + "." + nameof(Environment.OSVersion) + "." + nameof(Environment.OSVersion.ServicePack) + ": {0}", Environment.OSVersion.ServicePack);

      Console.WriteLine(nameof(HabitatInfo.OSName           ) + ": {0}", HabitatInfo.OSName);
      Console.WriteLine(nameof(HabitatInfo.ClrImplementation) + ": {0}", HabitatInfo.ClrImplementation);

      if (HabitatInfo.ClrImplementation == JetClrImplementation.Mono)
      {
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.MonoVersion), HabitatInfo.MonoVersion?.ToString() ?? "<null>");

        if (HabitatInfo.MonoVersion != null)
        {
          Assert.AreNotEqual(0, HabitatInfo.MonoVersion.Build);
          Assert.AreEqual(-1, HabitatInfo.MonoVersion.Revision);
        }
      }
      else
        Assert.IsNull(HabitatInfo.MonoVersion);

      Console.WriteLine(nameof(HabitatInfo.Platform              ) + ": {0}", HabitatInfo.Platform              );
      Console.WriteLine(nameof(HabitatInfo.ProcessArchitecture   ) + ": {0}", HabitatInfo.ProcessArchitecture   );
      Console.WriteLine(nameof(HabitatInfo.OSArchitecture        ) + ": {0}", HabitatInfo.OSArchitecture        );
      Console.WriteLine(nameof(HabitatInfo.ProcessRuntimeIdString) + ": {0}", HabitatInfo.ProcessRuntimeIdString);
      Console.WriteLine(nameof(HabitatInfo.OSRuntimeIdString     ) + ": {0}", HabitatInfo.OSRuntimeIdString     );

      Assert.AreEqual(HabitatInfo.ProcessArchitecture, HabitatInfo.GetProcessArchitecture(Process.GetCurrentProcess()));
      Assert.AreEqual(HabitatInfo.ProcessArchitecture, HabitatInfo.GetProcessArchitecture(Process.GetCurrentProcess().Id));

      if (HabitatInfo.Platform == JetPlatform.Windows)
      {
        Assert.AreEqual(PlatformID.Win32NT, Environment.OSVersion.Platform);

        Console.WriteLine(nameof(HabitatInfo.WindowsBuildNumber        ) + ": {0}", HabitatInfo.WindowsBuildNumber        ?.ToString() ?? "<null>");
        Console.WriteLine(nameof(HabitatInfo.WindowsInstallationType   ) + ": {0}", HabitatInfo.WindowsInstallationType   ?.ToString() ?? "<null>");
        Console.WriteLine(nameof(HabitatInfo.WindowsIsUserAdministrator) + ": {0}", HabitatInfo.WindowsIsUserAdministrator?.ToString() ?? "<null>");
        Console.WriteLine(nameof(HabitatInfo.WindowsIsElevated         ) + ": {0}", HabitatInfo.WindowsIsElevated         ?.ToString() ?? "<null>");
        Console.WriteLine(nameof(HabitatInfo.WindowsElevationType      ) + ": {0}", HabitatInfo.WindowsElevationType      ?.ToString() ?? "<null>");

        Assert.IsNotNull(HabitatInfo.WindowsBuildNumber);
        Assert.IsNotNull(HabitatInfo.WindowsIsUserAdministrator);
        Assert.IsNotNull(HabitatInfo.WindowsIsElevated);
        Assert.IsNotNull(HabitatInfo.WindowsElevationType);

        Assert.AreEqual(Environment.OSVersion.Version.Build, checked((int)HabitatInfo.WindowsBuildNumber!));
      }
      else
      {
        Assert.AreNotEqual(PlatformID.Win32NT, Environment.OSVersion.Platform);

        Assert.IsNull(HabitatInfo.WindowsInstallationType);
        Assert.IsNull(HabitatInfo.WindowsIsUserAdministrator);
        Assert.IsNull(HabitatInfo.WindowsIsElevated);
        Assert.IsNull(HabitatInfo.WindowsElevationType);
      }

      if (HabitatInfo.Platform == JetPlatform.MacOsX)
      {
        Console.WriteLine(nameof(HabitatInfo.MacOSVersion) + ": {0}", HabitatInfo.MacOSVersion?.ToString() ?? "<null>");

        if (HabitatInfo.MacOSVersion != null)
        {
          Assert.AreNotEqual(0, HabitatInfo.MacOSVersion.Build);
          Assert.AreEqual(-1, HabitatInfo.MacOSVersion.Revision);
        }
        else
          Assert.Fail();
      }
      else
        Assert.IsNull(HabitatInfo.MacOSVersion);

      if (HabitatInfo.Platform == JetPlatform.Linux)
      {
        Console.WriteLine(nameof(HabitatInfo.LinuxLibC       ) + ": {0}", HabitatInfo.LinuxLibC       ?.ToString() ?? "<null>");
        Console.WriteLine(nameof(HabitatInfo.LinuxLibCVersion) + ": {0}", HabitatInfo.LinuxLibCVersion?.ToString() ?? "<null>");
        Assert.IsNotNull(HabitatInfo.LinuxLibC);
        if (HabitatInfo.LinuxLibCVersion != null)
        {
          Assert.AreNotEqual(0, HabitatInfo.LinuxLibCVersion.Build);
          Assert.AreEqual(-1, HabitatInfo.LinuxLibCVersion.Revision);
        }
        else if (HabitatInfo.LinuxLibC == JetLinuxLibC.Glibc)
          Assert.Fail();
      }
      else
      {
        Assert.IsNull(HabitatInfo.LinuxLibC);
        Assert.IsNull(HabitatInfo.LinuxLibCVersion);
      }

      var properties = OsReleaseProperties.ReadFromDefaultLocations();
      if (HabitatInfo.Platform is JetPlatform.Linux or JetPlatform.FreeBSD)
      {
        Assert.IsNotNull(properties);

        Console.WriteLine("os-release[" + OsReleaseProperties.IdKey         + "]: {0}", properties!.TryGetValue(OsReleaseProperties.IdKey        ) ?? "<null>");
        Console.WriteLine("os-release[" + OsReleaseProperties.NameKey       + "]: {0}", properties!.TryGetValue(OsReleaseProperties.NameKey      ) ?? "<null>");
        Console.WriteLine("os-release[" + OsReleaseProperties.VersionIdKey  + "]: {0}", properties!.TryGetValue(OsReleaseProperties.VersionIdKey ) ?? "<null>");
        Console.WriteLine("os-release[" + OsReleaseProperties.PrettyNameKey + "]: {0}", properties!.TryGetValue(OsReleaseProperties.PrettyNameKey) ?? "<null>");

        Console.WriteLine("{0}: {1}", "UnixId", properties.TryGetUnixId()?.ToString() ?? "<null>");
      }
      else
        Assert.IsNull(properties);
    }
  }
}