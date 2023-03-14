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
    // @formatter:on
    [Test]
    public void ConvertToLibCTest(JetLinuxLibC expectedLinuxLibC, string interpreter) => Assert.AreEqual(expectedLinuxLibC, LinuxHelper.ConvertToLibC(interpreter));

    [TestCase("/lib/ld-uClibc.so.0")]
    [TestCase("/lib/ld.so.1")]
    [TestCase("/lib/ld64.so.1")]
    [TestCase("/lib64/ld64.so.2")]
    [TestCase("/libexec/ld-elf.so.1")]
    [TestCase("/usr/libexec/ld.so")]
    [Test]
    public void ConvertToLibCErrorTest(string interpreter) => Assert.Throws<PlatformNotSupportedException>(() => LinuxHelper.ConvertToLibC(interpreter));
  }
}