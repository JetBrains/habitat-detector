using System;
using JetBrains.FormatRipper.Elf;
using JetBrains.HabitatDetector.Impl.Linux;
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
    // @formatter:on
    [Test]
    public void ConvertToArchitectureTest(JetArchitecture expectedArchitecture, ELFCLASS eiClass, ELFDATA eiData, EM eMachine) => Assert.AreEqual(expectedArchitecture, LinuxHelper.ConvertToArchitecture(eiClass, eiData, eMachine));

    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2MSB, EM.EM_ARM)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_ARM)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_AARCH64)]
    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_AARCH64)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_LOONGARCH)]
    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_LOONGARCH)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_PPC64)]
    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_PPC64)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_S390)]
    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2MSB, EM.EM_S390)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2MSB, EM.EM_X86_64)]
    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2LSB, EM.EM_X86_64)]
    [TestCase(ELFCLASS.ELFCLASS32, ELFDATA.ELFDATA2MSB, EM.EM_386)]
    [TestCase(ELFCLASS.ELFCLASS64, ELFDATA.ELFDATA2LSB, EM.EM_386)]
    [Test]
    public void ConvertToArchitectureErrorTest(ELFCLASS eiClass, ELFDATA eiData, EM eMachine) => Assert.Throws<PlatformNotSupportedException>(() => LinuxHelper.ConvertToArchitecture(eiClass, eiData, eMachine));

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
    [TestCase(JetLinuxLibC.Glibc , "ld-linux-x86-64.so.2")]
    [TestCase(JetLinuxLibC.Glibc , "/snap/dotnet-sdk/current/lib/arm-linux-gnueabihf/ld-2.23.so")]
    [TestCase(JetLinuxLibC.Glibc , "/snap/dotnet-sdk/current/lib/x86_64-linux-gnu/ld-2.27.so")]
    [TestCase(JetLinuxLibC.Glibc , "arm-linux-gnueabihf/ld-2.23.so")]
    [TestCase(JetLinuxLibC.Musl  , "/lib/ld-musl-powerpc64le.so.1")]
    [TestCase(JetLinuxLibC.Musl  , "/lib/ld-musl-x86_64.so.1")]
    [TestCase(JetLinuxLibC.Musl  , "/snap/dotnet-sdk/current/lib/arm-linux-musleabihf/ld-2.23.so")]
    [TestCase(JetLinuxLibC.Musl  , "/snap/dotnet-sdk/current/lib/x86_64-linux-musl/ld-2.27.so")]
    [TestCase(null               , "/lib/ld-uClibc.so.0")]
    [TestCase(null               , "/lib/ld.so.1")]
    [TestCase(null               , "/lib/ld64.so.1")]
    [TestCase(null               , "/lib64/ld64.so.2")]
    [TestCase(null               , "/libexec/ld-elf.so.1")]
    [TestCase(null               , "/usr/libexec/ld.so")]
    // @formatter:on
    [Test]
    public void ConvertToLibCTest(JetLinuxLibC? expectedLinuxLibC, string interpreter)
    {
      Assert.AreEqual(expectedLinuxLibC, LinuxHelper.ConvertToLibC(interpreter));
    }

    // @formatter:off
    [TestCase("busybox.alpine-s390x"    , JetArchitecture.S390x  , JetLinuxLibC.Musl)]
    [TestCase("coreutils.nixos-aarch64" , JetArchitecture.Arm64  , JetLinuxLibC.Glibc)]
    [TestCase("coreutils.nixos-x86_64"  , JetArchitecture.X64    , JetLinuxLibC.Glibc)]
    [TestCase("mktemp.ubuntu-riscv64"   , JetArchitecture.RiscV64, JetLinuxLibC.Glibc)]
    [TestCase("nologin.opensuse-ppc64le", JetArchitecture.Ppc64le, JetLinuxLibC.Glibc)]
    // @formatter:on
    [Test]
    public void ElfTest(string resourceName, JetArchitecture expectedArchitecture, JetLinuxLibC expectedLinuxLibC)
    {
      var elfInfo = ResourceUtil.OpenRead(ResourceCategory.Elf, resourceName, LinuxHelper.GetElfInfo);
      Assert.AreEqual(expectedArchitecture, elfInfo.ProcessArchitecture);
      Assert.AreEqual(expectedLinuxLibC, elfInfo.LinuxLibC);
    }

    [TestCase("ldd (GNU libc) 2.14.1\nCopyright (C) 2011 Free Software Foundation, Inc.\n", "2.14.1")]
    [TestCase("ldd (GNU libc) 2.17\nCopyright (C) 2012 Free Software Foundation, Inc.\n", "2.17")] // centos 7
    [TestCase("ldd (Debian GLIBC 2.24-11+deb9u4) 2.24\nCopyright (C) 2016 Free Software Foundation, Inc.\n", "2.24")] // debian 9
    [TestCase("ldd (Ubuntu GLIBC 2.35-0ubuntu3.8) 2.35\nCopyright (C) 2022 Free Software Foundation, Inc.\n", "2.35")] // ubuntu 22.04
    [TestCase("ldd (Gentoo 2.29-r2 p3) 2.29\nCopyright (C) 2019 Free Software Foundation, Inc.\n", "2.29")]
    [TestCase("ldd (Gentoo 2.39-r6 (patchset 6)) 2.39\nCopyright (C) 2024 Free Software Foundation, Inc.\n", "2.39")] // gentoo stage3 2.15
    [Test]
    public void TryParseGlibcLddOutputTest(string output, string expectedVersionStr)
    {
      Assert.AreEqual(new Version(expectedVersionStr), LinuxHelper.ParseGlibcLddOutput(output));
    }

    [TestCase("musl libc (x86_64)\nVersion 1.2.4\nDynamic Program Loader\n", "1.2.4")]
    [TestCase("musl libc (aarch64)\nVersion 1.2.2\nDynamic Program Loader\n", "1.2.2")]
    [TestCase("musl libc (armhf)\nVersion 1.2.4_git20230717\nDynamic Program Loader\n", "1.2.4")] // alpine 3.19.1
    [Test]
    public void TryParseMuslLddOutputTest(string output, string expectedVersionStr)
    {
      Assert.AreEqual(new Version(expectedVersionStr), LinuxHelper.ParseMuslLddOutput(output));
    }

    [Platform("Linux")]
    [Test]
    public void ParseLibCVersionTest()
    {
      switch (HabitatInfo.LinuxLibC)
      {
      case JetLinuxLibC.Glibc:
        var glibcApiVersion = LinuxHelper.GetGlibcApiVersion();
        var glibcLddVersion = LinuxHelper.GetGlibcLddVersion(LinuxHelper.DefaultLdd);
        Console.WriteLine("GlibcApiVersion: {0}", glibcApiVersion);
        Console.WriteLine("GlibcLddVersion: {0}", glibcLddVersion?.ToString() ?? "<null>");
        if (glibcLddVersion != null)
          Assert.AreEqual(glibcApiVersion, glibcLddVersion);
        break;
      case JetLinuxLibC.Musl:
        var interpreter = LinuxHelper.GetElfInfo().Interpreter;
        var muslInterpreterVersion = LinuxHelper.GetMuslLddVersion(interpreter);
        var muslLddVersion = LinuxHelper.GetMuslLddVersion(LinuxHelper.DefaultLdd);
        Console.WriteLine("MuslLddVersion ({1}): {0}", muslInterpreterVersion?.ToString() ?? "<null>", interpreter);
        Console.WriteLine("MuslLddVersion ({1}): {0}", muslLddVersion?.ToString() ?? "<null>", LinuxHelper.DefaultLdd);
        Assert.AreEqual(muslLddVersion, muslInterpreterVersion);
        break;
      }
    }
  }
}