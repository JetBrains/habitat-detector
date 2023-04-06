using System;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class HabitatInfoTest
  {
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

    [TestCase(JetPlatform.Linux, JetArchitecture.Arm, JetLinuxLibC.Glibc, "linux-arm")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm, JetLinuxLibC.Musl, "linux-musl-arm")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm64, JetLinuxLibC.Glibc, "linux-arm64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Arm64, JetLinuxLibC.Musl, "linux-musl-arm64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.LoongArch64, JetLinuxLibC.Glibc, "linux-loongarch64")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Ppc64le, JetLinuxLibC.Glibc, "linux-ppc64le")]
    [TestCase(JetPlatform.Linux, JetArchitecture.Ppc64le, JetLinuxLibC.Musl, "linux-musl-ppc64le")]
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
      Check(JetPlatform.Linux, JetArchitecture.X64, JetArchitecture.X64);
      Check(JetPlatform.Linux, JetArchitecture.X86, JetArchitecture.X86);

      Check(JetPlatform.MacOsX, JetArchitecture.Arm64, JetArchitecture.Arm64, JetArchitecture.X64);
      Check(JetPlatform.MacOsX, JetArchitecture.X64, JetArchitecture.X64);

      Check(JetPlatform.Windows, JetArchitecture.Arm64, JetArchitecture.Arm64, JetArchitecture.X64, JetArchitecture.X86);
      Check(JetPlatform.Windows, JetArchitecture.X64, JetArchitecture.X64, JetArchitecture.X86);
      Check(JetPlatform.Windows, JetArchitecture.X86, JetArchitecture.X86);
    }

    [Test]
    public void CurrentTest()
    {
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.Platform), HabitatInfo.Platform);
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.ProcessArchitecture), HabitatInfo.ProcessArchitecture);
      Console.WriteLine("{0}: {1}", nameof(HabitatInfo.OSArchitecture), HabitatInfo.OSArchitecture);

      if (HabitatInfo.Platform == JetPlatform.Linux)
      {
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.LinuxLibC), HabitatInfo.LinuxLibC != null ? HabitatInfo.LinuxLibC.Value : "<null>");
        Console.WriteLine("{0}: {1}", nameof(HabitatInfo.LinuxDistro), HabitatInfo.LinuxDistro != null ? HabitatInfo.LinuxDistro.Value : "<null>");
        Assert.IsNotNull(HabitatInfo.LinuxLibC);
      }
      else
      {
        Assert.IsNull(HabitatInfo.LinuxLibC);
        Assert.IsNull(HabitatInfo.LinuxDistro);
      }
    }
  }
}