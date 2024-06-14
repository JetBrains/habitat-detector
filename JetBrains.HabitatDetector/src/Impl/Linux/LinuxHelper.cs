using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.FormatRipper.Elf;
using JetBrains.HabitatDetector.Impl.Unix;

namespace JetBrains.HabitatDetector.Impl.Linux
{
  internal static class LinuxHelper
  {
    internal static ElfInfo GetElfInfo() => GetElfInfo("/proc/self/exe");

    internal static ElfInfo GetElfInfo(string executable)
    {
      using var stream = File.OpenRead(executable);
      return GetElfInfo(stream);
    }

    internal static ElfInfo GetElfInfo(Stream stream)
    {
      var elfFile = ElfFile.Parse(stream);
      if (elfFile.EiOsAbi is not (ELFOSABI.ELFOSABI_NONE or ELFOSABI.ELFOSABI_LINUX))
        throw new FormatException($"Invalid ELF OS ABI identification {elfFile.EiOsAbi}");
      if (elfFile.EType is not (ET.ET_DYN or ET.ET_EXEC))
        throw new FormatException($"Invalid ELF object file type {elfFile.EType}");
      return new ElfInfo(
        ConvertToLibC(elfFile.Interpreter ?? throw new FormatException("Can't find ELF program interpreter")) ?? JetLinuxLibC.Glibc, // Note(ww898): Fallback to GLibC because OpenSUSE and other.
        ConvertToArchitecture(elfFile.EiClass, elfFile.EiData, elfFile.EMachine));
    }

    internal static JetLinuxLibC? ConvertToLibC(string interpreter)
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
      return null;
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
        case EM.EM_RISCV: return JetArchitecture.RiscV64;
        case EM.EM_X86_64: return JetArchitecture.X64;
        }
      else if (eiClass == ELFCLASS.ELFCLASS64 && eiData == ELFDATA.ELFDATA2MSB)
        switch (eMachine)
        {
        case EM.EM_S390: return JetArchitecture.S390x;
        }

      throw new PlatformNotSupportedException($"Invalid ELF file class {eiClass}, endian {eiData} and architecture {eMachine}");
    }

    internal record struct ElfInfo(JetLinuxLibC LinuxLibC, JetArchitecture ProcessArchitecture);

    private static string? RunLddVersion(bool shouldFail)
    {
#if NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
      return null;
#else
      const string ldd = "/usr/bin/ldd";

      // Note(ww898): We are fail on NixOS because no /usr/bin/ldd on system.
      if (!File.Exists(ldd))
        return null;

      var builder = new StringBuilder();

      void OnDataReceived(string? str)
      {
        if (str != null)
          lock (builder)
            builder.AppendLine(str);
      }

      using (var process = new Process())
      {
        // Note(ww898): MUSL doesn't support `--version` argument, so /usr/bin/ldd dumps version and fail with exit code 1, see https://github.com/kraj/musl/blob/007997299248b8682dcbb73595c53dfe86071c83/ldso/dynlink.c#L1895-L1901
        process.StartInfo = new ProcessStartInfo(ldd, "--version")
          {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = shouldFail
          };

        process.OutputDataReceived += (_, args) => OnDataReceived(args.Data);
        if (shouldFail)
          process.ErrorDataReceived += (_, args) => OnDataReceived(args.Data);

        if (!process.Start())
          throw new InvalidOperationException($"Failed to start {ldd} process");

        process.BeginOutputReadLine();
        if (shouldFail)
          process.BeginErrorReadLine();

        process.WaitForExit();

        if (process.ExitCode != 0 && (!shouldFail || process.ExitCode != 1))
          throw new InvalidOperationException($"The {ldd} process failed with exit code {process.ExitCode}");
      }
      return builder.ToString();
#endif
    }

    internal static Version GetGlibcApiVersion() => new(Marshal.PtrToStringAnsi(LibC.gnu_get_libc_version())!);

    internal static Version? GetGlibcLddVersion()
    {
        var output = RunLddVersion(false);
        return output != null ? ParseGlibcLddOutput(output) : null;
    }

    internal static Version? GetMuslLddVersion()
    {
      var output = RunLddVersion(true);
      return output != null ? ParseMuslLddOutput(output) : null;
    }

    internal static Version ParseGlibcLddOutput(string output)
    {
      var regex = new Regex(@"^ldd\s\((GNU\slibc|.+\sGLIBC\s.+)\)\s(?<version>\d+\.\d+(\.\d+)?)\n", RegexOptions.Singleline);
      var grp = regex.Match(output).Groups["version"];
      if (!grp.Success)
        throw new FormatException("Failed to parse GLibC version");
      return new Version(grp.Captures[0].Value);
    }

    internal static Version ParseMuslLddOutput(string output)
    {
      var regex = new Regex(@"^musl\slibc\s\(.+\)\nVersion\s(?<version>\d+\.\d+(\.\d+)?)\n", RegexOptions.Singleline);
      var grp = regex.Match(output).Groups["version"];
      if (!grp.Success)
        throw new FormatException("Failed to parse MUSL version");
      return new Version(grp.Captures[0].Value);
    }
  }
}