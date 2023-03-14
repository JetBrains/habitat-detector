using System.IO;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class OsReleaseTest
  {
    // @formatter:off
    [TestCase("alpine-3.17.3"     , "alpine"       , "3.17.3", "Alpine Linux v3.17"                  , JetUnixId.Alpine)]
    [TestCase("amzn-2"            , "amzn"         , "2"     , "Amazon Linux 2"                      , JetUnixId.Amazon)]
    [TestCase("elementary-5.1.4"  , "elementary"   , "5.1.4" , "elementary OS 5.1.4 Hera"            , JetUnixId.Elementary)]
    [TestCase("freebsd-13.1"      , "freebsd"      , "13.1"  , "FreeBSD 13.1-RELEASE-p7"             , JetUnixId.FreeBSD)]
    [TestCase("kali-2020.4"       , "kali"         , "2020.4", "Kali GNU/Linux Rolling"              , JetUnixId.Kali)]
    [TestCase("manjaro"           , "manjaro"      , null    , "Manjaro Ruah"                        , JetUnixId.Manjaro)]
    [TestCase("manjaro-arm"       , "manjaro-arm"  , null    , "Manjaro ARM"                         , JetUnixId.Manjaro)]
    [TestCase("opensuse-42.3"     , "opensuse"     , "42.3"  , "openSUSE Leap 42.3"                  , JetUnixId.OpenSUSE)]
    [TestCase("opensuse-leap-15.1", "opensuse-leap", "15.1"  , "openSUSE Leap 15.1"                  , JetUnixId.OpenSUSE)]
    [TestCase("rhel-8.4"          , "rhel"         , "8.4"   , "Red Hat Enterprise Linux 8.4 (Ootpa)", JetUnixId.RHEL)]
    [TestCase("ubuntu-22.04"      , "ubuntu"       , "22.04" , "Ubuntu 22.04.2 LTS"                  , JetUnixId.Ubuntu)]
    [TestCase("xxx"               , "ol'"          , "qwe"   , "asd"                                 , null)]
    // @formatter:on
    [Test]
    public void Test(string resourceName, string? expectedId, string? expectedVersionId, string? expectedPrettyName, JetUnixId? expectedUnixId)
    {
      var properties = ResourceUtil.OpenRead(ResourceCategory.OsRelease, resourceName, stream =>
        {
          using var reader = new StreamReader(stream);
          return OsReleaseProperties.ReadFromStream(reader);
        });
      Assert.AreEqual(expectedId, properties.TryGetValue(OsReleaseProperties.IdKey));
      Assert.AreEqual(expectedVersionId, properties.TryGetValue(OsReleaseProperties.VersionIdKey));
      Assert.AreEqual(expectedPrettyName, properties.TryGetValue(OsReleaseProperties.PrettyNameKey));
      
      Assert.AreEqual(expectedUnixId, properties.TryGetUnixId());
    }
  }
}