using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.FormatRipper.Elf;

namespace JetBrains.HabitatDetector.Impl.Linux
{
  internal static class LinuxHelper
  {
    internal static ElfInfo GetElfInfo()
    {
      ElfFile elfFile;
      using (var stream = File.OpenRead("/proc/self/exe"))
        elfFile = ElfFile.Parse(stream);

      if (elfFile.EiOsAbi is not (ELFOSABI.ELFOSABI_NONE or ELFOSABI.ELFOSABI_LINUX))
        throw new FormatException($"Invalid ELF OS ABI identification {elfFile.EiOsAbi}");
      if (elfFile.EType is not (ET.ET_DYN or ET.ET_EXEC))
        throw new FormatException($"Invalid ELF object file type {elfFile.EType}");

      return new ElfInfo(
        ConvertToLibC(elfFile.Interpreter ?? throw new FormatException("Can't find ELF program interpreter")),
        ConvertToArchitecture(elfFile.EiClass, elfFile.EiData, elfFile.EMachine));
    }

    internal static JetLinuxLibC ConvertToLibC(string interpreter)
    {
      if (interpreter is "/system/bin/linker64" or "/system/bin/linker")
        return JetLinuxLibC.Bionic;

      // Note(ww898,k15tfu): Removing interpreter directory is NixOS/snap support (see https://youtrack.jetbrains.com/issue/RIDER-55371):
      //   /nix/store/c1nqsqwl9allxbxhqx3iqfxk363qrnzv-glibc-2.32-54/lib/ld-linux-aarch64.so.1
      //   /nix/store/jsp3h3wpzc842j0rz61m5ly71ak6qgdn-glibc-2.32-54/lib/ld-linux-x86-64.so.2
      //   /snap/core18/current/lib64/ld-linux-x86-64.so.2
      //   /snap/core20/current/lib/ld-linux-aarch64.so.1
      //   /snap/core20/current/lib/ld-linux-armhf.so.3
      //   /snap/core18/current/lib/ld-linux.so.2
      //   /lib/ld-linux-aarch64.so.1
      //   /lib/ld-linux-armhf.so.3
      //   /lib/ld-linux.so.2
      //   /lib/ld-musl-aarch64.so.1
      //   /lib/ld-musl-armhf.so.1
      //   /lib/ld-musl-i386.so.1
      //   /lib/ld-musl-x86_64.so.1
      var n = interpreter.LastIndexOf('/');
      var interpreterFileName = interpreter.Substring(n + 1);
      if (interpreterFileName.StartsWith("ld-linux-") ||
          interpreterFileName.StartsWith("ld-linux."))
        return JetLinuxLibC.Glibc;
      if (interpreterFileName.StartsWith("ld-musl-") ||
          interpreterFileName.StartsWith("ld-musl."))
        return JetLinuxLibC.Musl;

      // Note(ww898,k15tfu): Something special for dotnet-sdk installed via new snap (see https://youtrack.jetbrains.com/issue/RIDER-80530):
      //   /snap/dotnet-sdk/current/lib/x86_64-linux-gnu/ld-2.27.so
      //   /lib/x86_64-linux-gnu/ld-2.27.so
      if (interpreterFileName.StartsWith("ld-") ||
          interpreterFileName.StartsWith("ld."))
      {
        // Note(ww898,k15tfu): See https://llvm.org/doxygen/Triple_8h_source.html / https://llvm.org/doxygen/Triple_8cpp_source.html
        //   x86_64-linux-gnu
        //   aarch64-linux-gnu
        //   arm-linux-gnueabihf
        //   aarch64-linux-musl
        //   arm-linux-musleabihf
        var k = interpreter.LastIndexOf('/', n - 1, n);
        var triple = interpreter.Substring(k + 1, n - k - 1);
        if (triple.Contains("-linux-gnu"))
          return JetLinuxLibC.Glibc;
        if (triple.Contains("-linux-musl"))
          return JetLinuxLibC.Musl;
      }

      // Bug(ww898,k15tfu): Please don't call external processes like ldd to detect GLibC/Musl in OS because it's incorrect for the current process (see NixOS/snap/...)!!!
      throw new PlatformNotSupportedException($"Unknown ELF interpreter {interpreter}");
    }

    internal static JetArchitecture ConvertToArchitecture(ELFCLASS eiClass, ELFDATA eiData, EM eMachine)
    {
      if (eiClass == ELFCLASS.ELFCLASS32 && eiData == ELFDATA.ELFDATA2LSB)
        switch (eMachine)
        {
        case EM.EM_ARM: return JetArchitecture.Arm;
        case EM.EM_386: return JetArchitecture.X86;
        }
      else if (eiClass == ELFCLASS.ELFCLASS64 && eiData == ELFDATA.ELFDATA2LSB)
        switch (eMachine)
        {
        case EM.EM_AARCH64: return JetArchitecture.Arm64;
        case EM.EM_LOONGARCH: return JetArchitecture.LoongArch64;
        case EM.EM_PPC64: return JetArchitecture.Ppc64le;
        case EM.EM_X86_64: return JetArchitecture.X64;
        }
      else if (eiClass == ELFCLASS.ELFCLASS64 && eiData == ELFDATA.ELFDATA2MSB)
        switch (eMachine)
        {
        case EM.EM_S390: return JetArchitecture.S390x;
        }

      throw new PlatformNotSupportedException($"Invalid ELF file class {eiClass}, endian {eiData} and architecture {eMachine}");
    }

    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    internal readonly struct ElfInfo
    {
      private readonly JetLinuxLibC myLinuxLibC;
      private readonly JetArchitecture myProcessArchitecture;

      internal ElfInfo(JetLinuxLibC linuxLibC, JetArchitecture processArchitecture)
      {
        myLinuxLibC = linuxLibC;
        myProcessArchitecture = processArchitecture;
      }

      public void Deconstruct(out JetLinuxLibC? linuxLibC, out JetArchitecture processArchitecture)
      {
        linuxLibC = myLinuxLibC;
        processArchitecture = myProcessArchitecture;
      }
    }
  }
}