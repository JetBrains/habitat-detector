using System.IO;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class OsReleaseTest
  {
    // @formatter:off
    [TestCase("alpine-3.17.3"     , "alpine"       , "3.17.3", "Alpine Linux"            , "Alpine Linux v3.17"                  , JetUnixId.Alpine)]
    [TestCase("amzn-2"            , "amzn"         , "2"     , "Amazon Linux"            , "Amazon Linux 2"                      , JetUnixId.Amazon)]
    [TestCase("elementary-5.1.4"  , "elementary"   , "5.1.4" , "elementary OS"           , "elementary OS 5.1.4 Hera"            , JetUnixId.Elementary)]
    [TestCase("freebsd-13.1"      , "freebsd"      , "13.1"  , "FreeBSD"                 , "FreeBSD 13.1-RELEASE-p7"             , JetUnixId.FreeBSD)]
    [TestCase("kali-2020.4"       , "kali"         , "2020.4", "Kali GNU/Linux"          , "Kali GNU/Linux Rolling"              , JetUnixId.Kali)]
    [TestCase("manjaro"           , "manjaro"      , null    , "Manjaro Linux"           , "Manjaro Ruah"                        , JetUnixId.Manjaro)]
    [TestCase("manjaro-arm"       , "manjaro-arm"  , null    , "Manjaro-ARM"             , "Manjaro ARM"                         , JetUnixId.Manjaro)]
    [TestCase("nixos-24.05"       , "nixos"        , "24.05" , "NixOS"                   , "NixOS 24.05 (Uakari)"                , JetUnixId.NixOS)]
    [TestCase("opensuse-42.3"     , "opensuse"     , "42.3"  , "openSUSE Leap"           , "openSUSE Leap 42.3"                  , JetUnixId.OpenSUSE)]
    [TestCase("opensuse-leap-15.1", "opensuse-leap", "15.1"  , "openSUSE Leap"           , "openSUSE Leap 15.1"                  , JetUnixId.OpenSUSE)]
    [TestCase("rhel-8.4"          , "rhel"         , "8.4"   , "Red Hat Enterprise Linux", "Red Hat Enterprise Linux 8.4 (Ootpa)", JetUnixId.RHEL)]
    [TestCase("sles-15.4"         , "sles"         , "15.4"  , "SLES"                    , "SUSE Linux Enterprise Server 15 SP4" , JetUnixId.SLES)]
    [TestCase("ubuntu-22.04"      , "ubuntu"       , "22.04" , "Ubuntu"                  , "Ubuntu 22.04.2 LTS"                  , JetUnixId.Ubuntu)]
    [TestCase("xxx"               , "ol'"          , "qwe"   , null                      , "asd"                                 , null)]
    // @formatter:on
    [Test]
    public void Test(string resourceName, string? expectedId, string? expectedVersionId, string? expectedName, string? expectedPrettyName, JetUnixId? expectedUnixId)
    {
      var properties = ResourceUtil.OpenRead(ResourceCategory.OsRelease, resourceName, stream =>
        {
          using var reader = new StreamReader(stream);
          return OsReleaseProperties.ReadFromStream(reader);
        });
      Assert.AreEqual(expectedId, properties.TryGetValue(OsReleaseProperties.IdKey));
      Assert.AreEqual(expectedVersionId, properties.TryGetValue(OsReleaseProperties.VersionIdKey));
      Assert.AreEqual(expectedName, properties.TryGetValue(OsReleaseProperties.NameKey));
      Assert.AreEqual(expectedPrettyName, properties.TryGetValue(OsReleaseProperties.PrettyNameKey));

      Assert.AreEqual(expectedUnixId, properties.TryGetUnixId());
    }
  }
}