using System;
using JetBrains.FormatRipper.Elf;
using JetBrains.HabitatDetector.Impl.Linux;
using JetBrains.HabitatDetector.Impl.Unix;
using NUnit.Framework;

namespace JetBrains.HabitatDetector.Tests
{
  [TestFixture]
  public sealed class LinuxHelperTest
  {
    // @formatter:off
    [TestCase(JetArchitecture.Arm        , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_ARM)]
    [TestCase(JetArchitecture.Arm64      , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_AARCH64)]
    [TestCase(JetArchitecture.LoongArch64, ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_LOONGARCH)]
    [TestCase(JetArchitecture.Ppc64le    , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_PPC64)]
    [TestCase(JetArchitecture.RiscV64    , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_RISCV)]
    [TestCase(JetArchitecture.S390x      , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_S390)]
    [TestCase(JetArchitecture.X64        , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_X86_64)]
    [TestCase(JetArchitecture.X86        , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_386)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2MSB, EM.EM_ARM)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_ARM)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_AARCH64)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_AARCH64)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_LOONGARCH)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_LOONGARCH)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_PPC64)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_PPC64)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_S390)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2MSB, EM.EM_S390)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_X86_64)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_X86_64)]
    [TestCase(null                       , ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2MSB, EM.EM_386)]
    [TestCase(null                       , ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_386)]
    // @formatter:on
    [Test]
    public void ConvertToArchitectureErrorTest(JetArchitecture? expectedArchitecture, ELFCLASS eiClass, ELFDATA eiData, EM eMachine)
    {
      if (expectedArchitecture == null)
        Assert.Throws<PlatformNotSupportedException>(() => LinuxHelper.ConvertToArchitecture(eiClass, eiData, eMachine));
      else
        Assert.AreEqual(expectedArchitecture, LinuxHelper.ConvertToArchitecture(eiClass, eiData, eMachine));
    }

    // @formatter:off
    [TestCase(JetLinuxLibC.Bionic, "/system/bin/linker")]
    [TestCase(JetLinuxLibC.Bionic, "/system/bin/linker64")]
    [TestCase(JetLinuxLibC.Glibc , "/lib/ld-linux-aarch64.so.1")]
    [TestCase(JetLinuxLibC.Glibc , "/lib/ld-linux-ia64.so.2")]
    [TestCase(JetLinuxLibC.Glibc , "/lib/ld-linux.so.3")]
    [TestCase(JetLinuxLibC.Glibc , "/lib/x86_64-linux-gnu/ld-2.27.so")]
    [TestCase(JetLinuxLibC.Glibc , "/lib64/ld-linux-x86-64.so.2")]
    [TestCase(JetLinuxLibC.Glibc , "/nix/store/c1nqsqwl9allxbxhqx3iqfxk363qrnzv-glibc-2.32-54/lib/ld-linux-aarch64.so.1")]
    [TestCase(JetLinuxLibC.Glibc , "/nix/store/jsp3h3wpzc842j0rz61m5ly71ak6qgdn-glibc-2.32-54/lib/ld-linux-x86-64.so.2")]
    [TestCase(JetLinuxLibC.Glibc , "/snap/dotnet-sdk/current/lib/arm-linux-gnueabihf/ld-2.23.so")]
    [TestCase(JetLinuxLibC.Glibc , "/snap/dotnet-sdk/current/lib/x86_64-linux-gnu/ld-2.27.so")]
    [TestCase(JetLinuxLibC.Musl  , "/lib/ld-musl-powerpc64le.so.1")]
    [TestCase(JetLinuxLibC.Musl  , "/lib/ld-musl-x86_64.so.1")]
    [TestCase(JetLinuxLibC.Musl  , "/snap/dotnet-sdk/current/lib/arm-linux-musleabihf/ld-2.23.so")]
    [TestCase(JetLinuxLibC.Musl  , "/snap/dotnet-sdk/current/lib/x86_64-linux-musl/ld-2.27.so")]
    [TestCase(null               , "/lib/ld-uClibc.so.0")]
    [TestCase(JetLinuxLibC.Glibc , "/lib/ld.so.1")]
    [TestCase(JetLinuxLibC.Glibc , "/lib/ld64.so.1")]
    [TestCase(JetLinuxLibC.Glibc , "/lib64/ld64.so.2")]
    [TestCase(null               , "/libexec/ld-elf.so.1")]
    [TestCase(JetLinuxLibC.Glibc , "/usr/libexec/ld.so")]
    // @formatter:on
    [Test]
    public void ConvertToLibCTest(JetLinuxLibC? expectedLinuxLibC, string interpreter)
    {
      if (expectedLinuxLibC == null)
        Assert.Throws<PlatformNotSupportedException>(() => LinuxHelper.ConvertToLibC(interpreter));
      else
        Assert.AreEqual(expectedLinuxLibC, LinuxHelper.ConvertToLibC(interpreter));
    }

    // @formatter:off
    [TestCase("busybox.alpine-s390x"    , false, JetArchitecture.S390x  , JetLinuxLibC.Musl , "/lib/ld-musl-s390x.so.1")]
    [TestCase("coreutils.nixos-aarch64" , false, JetArchitecture.Arm64  , JetLinuxLibC.Glibc, "/nix/store/c1nqsqwl9allxbxhqx3iqfxk363qrnzv-glibc-2.32-54/lib/ld-linux-aarch64.so.1")]
    [TestCase("coreutils.nixos-x86_64"  , false, JetArchitecture.X64    , JetLinuxLibC.Glibc, "/nix/store/jsp3h3wpzc842j0rz61m5ly71ak6qgdn-glibc-2.32-54/lib/ld-linux-x86-64.so.2")]
    [TestCase("ld-musl-x86_64.so-1"     , true , JetArchitecture.X64    , JetLinuxLibC.Musl , "/lib/ld-musl-x86_64.so.1")]
    [TestCase("mktemp.ubuntu-riscv64"   , false, JetArchitecture.RiscV64, JetLinuxLibC.Glibc, "/lib/ld-linux-riscv64-lp64d.so.1")]
    [TestCase("nologin.opensuse-ppc64le", false, JetArchitecture.Ppc64le, JetLinuxLibC.Glibc, "/lib64/ld64.so.2")]
    // @formatter:on
    [Test]
    public void GetElfTest(string resourceName, bool useExecutableAsInterpreter, JetArchitecture expectedArchitecture, JetLinuxLibC expectedLinuxLibC, string expectedInterpreter)
    {
      var elfInfo = ResourceUtil.OpenRead(ResourceCategory.Elf, resourceName, stream => LinuxHelper.GetElfInfo(useExecutableAsInterpreter ? expectedInterpreter : "", stream));
      Assert.AreEqual(expectedArchitecture, elfInfo.ProcessArchitecture);
      Assert.AreEqual(expectedLinuxLibC, elfInfo.LinuxLibC);
      Assert.AreEqual(expectedInterpreter, elfInfo.Interpreter);
    }

    // @formatter:off
    [TestCase("2.14.1"   , "ldd (GNU libc) 2.14.1\nCopyright (C) 2011 Free Software Foundation, Inc.\n")]
    [TestCase("2.17"     , "ldd (GNU libc) 2.17\nCopyright (C) 2012 Free Software Foundation, Inc.\n")] // centos 7
    [TestCase("2.24"     , "ldd (Debian GLIBC 2.24-11+deb9u4) 2.24\nCopyright (C) 2016 Free Software Foundation, Inc.\n")] // debian 9
    [TestCase("2.35"     , "ldd (Ubuntu GLIBC 2.35-0ubuntu3.8) 2.35\nCopyright (C) 2022 Free Software Foundation, Inc.\n")] // ubuntu 22.04
    [TestCase("2.29"     , "ldd (Gentoo 2.29-r2 p3) 2.29\nCopyright (C) 2019 Free Software Foundation, Inc.\n")]
    [TestCase("2.39"     , "ldd (Gentoo 2.39-r6 (patchset 6)) 2.39\nCopyright (C) 2024 Free Software Foundation, Inc.\n")] // gentoo stage3 2.15
    [TestCase("2.39"     , "ld.so (Ubuntu GLIBC 2.39-0ubuntu8.6) stable release version 2.39.\nCopyright (C) 2024 Free Software Foundation, Inc.\n")] // ubuntu 24.04.3
    [TestCase("2.42.9000", "ld.so (Ubuntu GLIBC 2.39-0ubuntu8.6) development release version 2.42.9000.\nCopyright (C) 2024 Free Software Foundation, Inc.\n")] // ubuntu 24.04.3
    // @formatter:on
    [Test]
    public void ParseGlibcLddOutputTest(string expectedVersionStr, string output)
    {
      Assert.AreEqual(new Version(expectedVersionStr), LinuxHelper.ParseGlibcLddOutput(output));
    }

    [TestCase("1.2.4", "musl libc (x86_64)\nVersion 1.2.4\nDynamic Program Loader\n")]
    [TestCase("1.2.2", "musl libc (aarch64)\nVersion 1.2.2\nDynamic Program Loader\n")]
    [TestCase("1.2.4", "musl libc (armhf)\nVersion 1.2.4_git20230717\nDynamic Program Loader\n")] // alpine 3.19.1
    [Test]
    public void ParseMuslLddOutputTest(string expectedVersionStr, string output)
    {
      Assert.AreEqual(new Version(expectedVersionStr), LinuxHelper.ParseMuslLddOutput(output));
    }

    [Platform("Linux")]
    [Test]
    public void GetElfInfoTest()
    {
      if (HabitatInfo.Platform != JetPlatform.Linux)
        Assert.Ignore("Linux test only");
      var (linuxLibC, processArchitecture, interpreter) = LinuxHelper.GetElfInfo(LinuxHelper.CurrentExecutable);
      Console.WriteLine("GetElfInfo ({3}): {0},{1},{2}", linuxLibC, processArchitecture, interpreter, LinuxHelper.CurrentExecutable);
    }

    [Platform("Linux")]
    [Test]
    public void GetRealPathTest()
    {
      if (HabitatInfo.Platform != JetPlatform.Linux)
        Assert.Ignore("Linux test only");
      Console.WriteLine("GetRealPath ({1}): {0}", UnixHelper.GetRealPath(LinuxHelper.CurrentExecutable), LinuxHelper.CurrentExecutable);
    }

    [Platform("Linux")]
    [Test]
    public void ParseLibCVersionTest()
    {
      if (HabitatInfo.Platform != JetPlatform.Linux)
        Assert.Ignore("Linux test only");
      switch (HabitatInfo.LinuxLibC)
      {
      case JetLinuxLibC.Glibc:
        var glibcApiVersion = LinuxHelper.GetGlibcApiVersion();
        Console.WriteLine("GlibcApiVersion: {0}", glibcApiVersion);
        foreach (var ldd in new[] { LinuxHelper.DefaultLdd, LinuxHelper.GetElfInfo(LinuxHelper.CurrentExecutable).Interpreter })
        {
          var lddVersion = LinuxHelper.GetGlibcLddVersion(ldd);
          Console.WriteLine("GetGlibcLddVersion ({1}): {0}", lddVersion?.ToString() ?? "<null>", ldd);
          Assert.AreEqual(glibcApiVersion, lddVersion);
        }
        break;
      case JetLinuxLibC.Musl:
        Version? muslLddVersion = null;
        foreach (var ldd in new[] { LinuxHelper.DefaultLdd, LinuxHelper.GetElfInfo(LinuxHelper.CurrentExecutable).Interpreter })
        {
          var lddVersion = LinuxHelper.GetMuslLddVersion(ldd);
          Console.WriteLine("GetMuslLddVersion ({1}): {0}", lddVersion?.ToString() ?? "<null>", ldd);
          if (muslLddVersion == null)
            muslLddVersion = lddVersion;
          else
            Assert.AreEqual(muslLddVersion, lddVersion);
        }
        Assert.IsNotNull(muslLddVersion);
        break;
      }
    }
  }
}