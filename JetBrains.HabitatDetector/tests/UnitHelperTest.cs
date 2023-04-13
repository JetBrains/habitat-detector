using JetBrains.HabitatDetector.Impl.Unix;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class UnitHelperTest
  {
    [TestCase("Darwin", JetPlatform.MacOsX, 256)]
    [TestCase("FreeBSD", JetPlatform.FreeBSD, 32)]
    [TestCase("Linux", JetPlatform.Linux, 65)]
    [Test]
    public void PlatformTest(string sysname, JetPlatform expectedPlatform, int expectedNameLength)
    {
      Assert.AreEqual(expectedPlatform, UnixHelper.ConvertToPlatform(sysname));
      Assert.AreEqual(expectedNameLength, UnixHelper.ConvertToNameLength(expectedPlatform));
    }

    [TestCase(JetPlatform.FreeBSD, "amd64", JetArchitecture.X64)]
    [TestCase(JetPlatform.FreeBSD, "arm64", JetArchitecture.Arm64)]
    [TestCase(JetPlatform.Linux, "aarch64", JetArchitecture.Arm64)]
    [TestCase(JetPlatform.Linux, "armv7l", JetArchitecture.Arm)]
    [TestCase(JetPlatform.Linux, "armv8l", JetArchitecture.Arm)]
    [TestCase(JetPlatform.Linux, "loongarch64", JetArchitecture.LoongArch64)]
    [TestCase(JetPlatform.Linux, "ppc64le", JetArchitecture.Ppc64le)]
    [TestCase(JetPlatform.Linux, "s390x", JetArchitecture.S390x)]
    [TestCase(JetPlatform.Linux, "x86_64", JetArchitecture.X64)]
    [TestCase(JetPlatform.MacOsX, "arm64", JetArchitecture.Arm64)]
    [TestCase(JetPlatform.MacOsX, "x86_64", JetArchitecture.X64)]
    [Test]
    public void ConvertToArchitecture(JetPlatform platform, string machine, JetArchitecture expectedArchitecture) => Assert.AreEqual(expectedArchitecture, UnixHelper.ConvertToArchitecture(platform, machine));
  }
}