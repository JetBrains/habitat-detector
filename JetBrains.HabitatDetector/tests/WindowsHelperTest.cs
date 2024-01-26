using System;
using JetBrains.HabitatDetector.Impl.Windows;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class WindowsHelperTest
  {
    [Platform("Win32NT")]
    [Test]
    public void BuildNumberTest()
    {
      Assert.AreEqual(JetPlatform.Windows, HabitatInfo.Platform);
      Assert.IsNotNull(HabitatInfo.WindowsBuildNumber);
      Assert.AreEqual(checked((uint)Environment.OSVersion.Version.Build), HabitatInfo.WindowsBuildNumber!.Value);
    }

    [Test]
    public void WinVistaTest()
    {
      Assert.AreEqual(WindowsHelper.FixProductName("Windows Vista (TM)", 6002), "Windows Vista");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows Vista (TM) Starter", 6002), "Windows Vista Starter");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows Vista (TM) Home Premium", 6002), "Windows Vista Home Premium");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows Vista (TM) Ultimate", 6002), "Windows Vista Ultimate");
    }

    [Test]
    public void WinNotModify()
    {
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 9", 10240), "Windows 9");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 99", 10240), "Windows 99");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 999", 10240), "Windows 999");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 9 Pro", 10240), "Windows 9 Pro");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 99 Pro", 10240), "Windows 99 Pro");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 999 Pro", 10240), "Windows 999 Pro");

      Assert.AreEqual(WindowsHelper.FixProductName("Windows 9", 22000), "Windows 9");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 99", 22000), "Windows 99");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 999", 22000), "Windows 999");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 9 Pro", 22000), "Windows 9 Pro");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 99 Pro", 22000), "Windows 99 Pro");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 999 Pro", 22000), "Windows 999 Pro");
    }

    [Test]
    public void Win10()
    {
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 10", 10240), "Windows 10");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 10 Pro", 10240), "Windows 10 Pro");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 10 Holographic", 10240), "Windows 10 Holographic");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows Holographic", 10240), "Windows 10 Holographic");
    }

    [Test]
    public void Win11()
    {
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 10", 22000), "Windows 11");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 10 Pro", 22000), "Windows 11 Pro");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows 10 Holographic", 22000), "Windows 11 Holographic");
      Assert.AreEqual(WindowsHelper.FixProductName("Windows Holographic", 22000), "Windows 11 Holographic");
    }

    [Test]
    public void WinSrvNotModify()
    {
      Assert.AreEqual("Windows Server 9", WindowsHelper.FixProductName("Windows Server 9", 14393));
      Assert.AreEqual("Windows Server 9999", WindowsHelper.FixProductName("Windows Server 9999", 14393));
      Assert.AreEqual("Windows Server 99999", WindowsHelper.FixProductName("Windows Server 99999", 14393));
      Assert.AreEqual("Windows Server 9 Datacenter", WindowsHelper.FixProductName("Windows Server 9 Datacenter", 14393));
      Assert.AreEqual("Windows Server 9999 Datacenter", WindowsHelper.FixProductName("Windows Server 9999 Datacenter", 14393));
      Assert.AreEqual("Windows Server 99999 Datacenter", WindowsHelper.FixProductName("Windows Server 99999 Datacenter", 14393));

      Assert.AreEqual("Windows Server 9", WindowsHelper.FixProductName("Windows Server 9", 20348));
      Assert.AreEqual("Windows Server 9999", WindowsHelper.FixProductName("Windows Server 9999", 20348));
      Assert.AreEqual("Windows Server 99999", WindowsHelper.FixProductName("Windows Server 99999", 20348));
      Assert.AreEqual("Windows Server 9 Datacenter", WindowsHelper.FixProductName("Windows Server 9 Datacenter", 20348));
      Assert.AreEqual("Windows Server 9999 Datacenter", WindowsHelper.FixProductName("Windows Server 9999 Datacenter", 20348));
      Assert.AreEqual("Windows Server 99999 Datacenter", WindowsHelper.FixProductName("Windows Server 99999 Datacenter", 20348));

      Assert.AreEqual("Windows Fake Server 9", WindowsHelper.FixProductName("Windows Fake Server 9", 14393));
      Assert.AreEqual("Windows Fake Server 9999", WindowsHelper.FixProductName("Windows Fake Server 9999", 14393));
      Assert.AreEqual("Windows Fake Server 99999", WindowsHelper.FixProductName("Windows Fake Server 99999", 14393));
      Assert.AreEqual("Windows Fake Server 9 Datacenter", WindowsHelper.FixProductName("Windows Fake Server 9 Datacenter", 14393));
      Assert.AreEqual("Windows Fake Server 9999 Datacenter", WindowsHelper.FixProductName("Windows Fake Server 9999 Datacenter", 14393));
      Assert.AreEqual("Windows Fake Server 99999 Datacenter", WindowsHelper.FixProductName("Windows Fake Server 99999 Datacenter", 14393));

      Assert.AreEqual("Windows ExtraFake Server 9", WindowsHelper.FixProductName("Windows ExtraFake Server 9", 20348));
      Assert.AreEqual("Windows ExtraFake Server 9999", WindowsHelper.FixProductName("Windows ExtraFake Server 9999", 20348));
      Assert.AreEqual("Windows ExtraFake Server 99999", WindowsHelper.FixProductName("Windows ExtraFake Server 99999", 20348));
      Assert.AreEqual("Windows ExtraFake Server 9 Datacenter", WindowsHelper.FixProductName("Windows ExtraFake Server 9 Datacenter", 20348));
      Assert.AreEqual("Windows ExtraFake Server 9999 Datacenter", WindowsHelper.FixProductName("Windows ExtraFake Server 9999 Datacenter", 20348));
      Assert.AreEqual("Windows ExtraFake Server 99999 Datacenter", WindowsHelper.FixProductName("Windows ExtraFake Server 99999 Datacenter", 20348));
    }

    [Test]
    public void WinSrv2016()
    {
      Assert.AreEqual("Windows Server 2016", WindowsHelper.FixProductName("Windows Server 2016", 14393));
      Assert.AreEqual("Windows Server 2016", WindowsHelper.FixProductName("Windows Server", 14393));
      Assert.AreEqual("Windows Server 2016 Datacenter", WindowsHelper.FixProductName("Windows Server 2016 Datacenter", 14393));
      Assert.AreEqual("Windows Server 2016 Datacenter", WindowsHelper.FixProductName("Windows Server Datacenter", 14393));

      Assert.AreEqual("Windows MultiPoint Server 2016", WindowsHelper.FixProductName("Windows MultiPoint Server 2016", 14393));
      Assert.AreEqual("Windows MultiPoint Server 2016", WindowsHelper.FixProductName("Windows MultiPoint Server", 14393));
      Assert.AreEqual("Windows MultiPoint Server 2016 Datacenter", WindowsHelper.FixProductName("Windows MultiPoint Server 2016 Datacenter", 14393));
      Assert.AreEqual("Windows MultiPoint Server 2016 Datacenter", WindowsHelper.FixProductName("Windows MultiPoint Server Datacenter", 14393));
    }

    [Test]
    public void WinSrv2019()
    {
      Assert.AreEqual("Windows Server 2019", WindowsHelper.FixProductName("Windows Server 2019", 17763));
      Assert.AreEqual("Windows Server 2019", WindowsHelper.FixProductName("Windows Server", 17763));
      Assert.AreEqual("Windows Server 2019 Datacenter", WindowsHelper.FixProductName("Windows Server 2019 Datacenter", 17763));
      Assert.AreEqual("Windows Server 2019 Datacenter", WindowsHelper.FixProductName("Windows Server Datacenter", 17763));

      Assert.AreEqual("Windows MultiPoint Server 2019", WindowsHelper.FixProductName("Windows MultiPoint Server 2019", 17763));
      Assert.AreEqual("Windows MultiPoint Server 2019", WindowsHelper.FixProductName("Windows MultiPoint Server", 17763));
      Assert.AreEqual("Windows MultiPoint Server 2019 Datacenter", WindowsHelper.FixProductName("Windows MultiPoint Server 2019 Datacenter", 17763));
      Assert.AreEqual("Windows MultiPoint Server 2019 Datacenter", WindowsHelper.FixProductName("Windows MultiPoint Server Datacenter", 17763));
    }

    [Test]
    public void WinSrv2022()
    {
      Assert.AreEqual("Windows Server 2022", WindowsHelper.FixProductName("Windows Server 2022", 20348));
      Assert.AreEqual("Windows Server 2022", WindowsHelper.FixProductName("Windows Server", 20348));
      Assert.AreEqual("Windows Server 2022 Datacenter", WindowsHelper.FixProductName("Windows Server 2022 Datacenter", 20348));
      Assert.AreEqual("Windows Server 2022 Datacenter", WindowsHelper.FixProductName("Windows Server Datacenter", 20348));

      Assert.AreEqual("Windows MultiPoint Server 2022", WindowsHelper.FixProductName("Windows MultiPoint Server 2022", 20348));
      Assert.AreEqual("Windows MultiPoint Server 2022", WindowsHelper.FixProductName("Windows MultiPoint Server", 20348));
      Assert.AreEqual("Windows MultiPoint Server 2022 Datacenter", WindowsHelper.FixProductName("Windows MultiPoint Server 2022 Datacenter", 20348));
      Assert.AreEqual("Windows MultiPoint Server 2022 Datacenter", WindowsHelper.FixProductName("Windows MultiPoint Server Datacenter", 20348));
    }
  }
}