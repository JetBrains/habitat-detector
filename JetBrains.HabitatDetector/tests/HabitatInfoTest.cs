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
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.ClrImplementation), HabitatInfo.ClrImplementation);

      if (HabitatInfo.ClrImplementation == JetClrImplementation.Mono)
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.MonoVersion), HabitatInfo.MonoVersion?.ToString() ?? "<null>");
      else
        Assert.IsNull(HabitatInfo.MonoVersion);

      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.Platform), HabitatInfo.Platform);
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.ProcessArchitecture), HabitatInfo.ProcessArchitecture);
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.OSArchitecture), HabitatInfo.OSArchitecture);

      Assert.AreEqual(HabitatInfo.ProcessArchitecture, HabitatInfo.GetProcessArchitecture(Process.GetCurrentProcess()));
      Assert.AreEqual(HabitatInfo.ProcessArchitecture, HabitatInfo.GetProcessArchitecture(Process.GetCurrentProcess().Id));

      if (HabitatInfo.Platform == JetPlatform.Windows)
      {
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.WindowsInstallationType), HabitatInfo.WindowsInstallationType?.ToString() ?? "<null>");
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.WindowsIsUserAdministrator), HabitatInfo.WindowsIsUserAdministrator?.ToString() ?? "<null>");
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.WindowsIsElevated), HabitatInfo.WindowsIsElevated?.ToString() ?? "<null>");
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.WindowsElevationType), HabitatInfo.WindowsElevationType?.ToString() ?? "<null>");

        Assert.IsNotNull(HabitatInfo.WindowsIsUserAdministrator);
        Assert.IsNotNull(HabitatInfo.WindowsIsElevated);
        Assert.IsNotNull(HabitatInfo.WindowsElevationType);
      }
      else
      {
        Assert.IsNull(HabitatInfo.WindowsInstallationType);
        Assert.IsNull(HabitatInfo.WindowsIsUserAdministrator);
        Assert.IsNull(HabitatInfo.WindowsIsElevated);
        Assert.IsNull(HabitatInfo.WindowsElevationType);
      }

      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.ProcessRuntimeIdString), HabitatInfo.ProcessRuntimeIdString);
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.OSRuntimeIdString), HabitatInfo.OSRuntimeIdString);

      if (HabitatInfo.Platform == JetPlatform.Linux)
      {
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.LinuxLibC), HabitatInfo.LinuxLibC?.ToString() ?? "<null>");
        Assert.IsNotNull(HabitatInfo.LinuxLibC);
      }
      else
        Assert.IsNull(HabitatInfo.LinuxLibC);

      var properties = OsReleaseProperties.ReadFromDefaultLocations();
      if (HabitatInfo.Platform is JetPlatform.Linux or JetPlatform.FreeBSD)
      {
        Assert.IsNotNull(properties);

        Console.WriteLine("{0}: {1}", "os-release[" + OsReleaseProperties.IdKey         + ']', properties!.TryGetValue(OsReleaseProperties.IdKey        ) ?? "<null>");
        Console.WriteLine("{0}: {1}", "os-release[" + OsReleaseProperties.VersionIdKey  + ']', properties!.TryGetValue(OsReleaseProperties.VersionIdKey ) ?? "<null>");
        Console.WriteLine("{0}: {1}", "os-release[" + OsReleaseProperties.PrettyNameKey + ']', properties!.TryGetValue(OsReleaseProperties.PrettyNameKey) ?? "<null>");

        Console.WriteLine("{0}: {1}", "UnixId", properties.TryGetUnixId()?.ToString() ?? "<null>");
      }
      else
        Assert.IsNull(properties);
    }
  }
}