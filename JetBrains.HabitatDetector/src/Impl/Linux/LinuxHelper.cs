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
    internal const string DefaultLdd = "/usr/bin/ldd";
    internal const string DefaultExecutable = "/proc/self/exe";

    internal static ElfInfo GetElfInfo(string executable)
    {
      using var stream = File.OpenRead(executable);
      return GetElfInfo(UnixHelper.GetRealPath(executable), stream);
    }

    internal static ElfInfo GetElfInfo(string executable, Stream stream)
    {
      var elfFile = ElfFile.Parse(stream);
      if (elfFile.EiOsAbi is not (ELFOSABI.ELFOSABI_NONE or ELFOSABI.ELFOSABI_LINUX))
        throw new FormatException($"Invalid ELF OS ABI identification {elfFile.EiOsAbi} in executable {executable}");
      if (elfFile.EType is not (ET.ET_DYN or ET.ET_EXEC))
        throw new FormatException($"Invalid ELF object file type {elfFile.EType} in executable {executable}");
      var interpreter = elfFile.Interpreter ?? executable;
      return new ElfInfo(
        ConvertToLibC(interpreter),
        ConvertToArchitecture(elfFile.EiClass, elfFile.EiData, elfFile.EMachine),
        interpreter);
    }

    internal static JetLinuxLibC ConvertToLibC(string interpreter)
    {
      if (interpreter.Length == 0 || interpreter[0] != '/')
        throw new FormatException($"Absolute path is expected in interpreter {interpreter}");

      if (interpreter is "/system/bin/linker64" or "/system/bin/linker")
        return JetLinuxLibC.Bionic;

      var parts = interpreter.Substring(1).Split('/');
      var prevDirectoryName = parts.Length >= 3 ? parts[parts.Length - 3] : "";
      var directoryName = parts.Length >= 2 ? parts[parts.Length - 2] : "";
      var fileName = parts.Length >= 1 ? parts[parts.Length - 1] : "";

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
      if (directoryName.StartsWith("lib"))
        if (fileName.StartsWith("ld-linux-") ||
            fileName.StartsWith("ld-linux."))
          return JetLinuxLibC.Glibc;
        else if (fileName.StartsWith("ld-musl-") ||
                 fileName.StartsWith("ld-musl."))
          return JetLinuxLibC.Musl;

      // Note(ww898): Always GNU C library
      //   /lib/ld.so.1
      //   /lib/ld64.so.1
      //   /lib64/ld64.so.2
      //   /usr/libexec/ld.so
      //   /lib/ld-musl-x86_64.so.1
      if (directoryName.StartsWith("lib"))
        if (fileName.StartsWith("ld.so") ||
            fileName.StartsWith("ld64.so"))
          return JetLinuxLibC.Glibc;

      // Note(ww898,k15tfu): Something special for dotnet-sdk installed via a new snap (see https://youtrack.jetbrains.com/issue/RIDER-80530):
      //   /snap/dotnet-sdk/current/lib/x86_64-linux-gnu/ld-2.27.so
      //   /lib/x86_64-linux-gnu/ld-2.27.so
      //
      // Note(ww898,k15tfu): See https://llvm.org/doxygen/Triple_8h_source.html / https://llvm.org/doxygen/Triple_8cpp_source.html
      //   x86_64-linux-gnu
      //   aarch64-linux-gnu
      //   arm-linux-gnueabihf
      //   aarch64-linux-musl
      //   arm-linux-musleabihf
      if (prevDirectoryName.StartsWith("lib"))
        if (fileName.StartsWith("ld-") ||
            fileName.StartsWith("ld."))
          if (directoryName.Contains("-linux-gnu"))
            return JetLinuxLibC.Glibc;
          else if (directoryName.Contains("-linux-musl"))
            return JetLinuxLibC.Musl;

      // Bug(ww898,k15tfu): Please don't call external processes like ldd to detect GLibC/Musl in OS because it's incorrect for the current process (see NixOS/snap/...)!!!
      throw new PlatformNotSupportedException($"Can't parse the ELF program interpreter {interpreter}");
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

    internal record struct ElfInfo(JetLinuxLibC LinuxLibC, JetArchitecture ProcessArchitecture, string Interpreter);

    private static string? RunLddVersion(bool shouldFail, string ldd)
    {
#if NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
      return null;
#else
      // Note(ww898): We can fail whether no ldd on system.
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

    internal static Version? GetGlibcLddVersion(string ldd)
    {
      var output = RunLddVersion(false, ldd);
      return output != null ? ParseGlibcLddOutput(output) : null;
    }

    internal static Version? GetMuslLddVersion(string ldd)
    {
      var output = RunLddVersion(true, ldd);
      return output != null ? ParseMuslLddOutput(output) : null;
    }

    internal static Version ParseGlibcLddOutput(string output)
    {
      // Note(ww898): See https://sourceware.org/git/?p=glibc.git;a=blob;f=elf/ldd.bash.in;hb=HEAD#l38
      //              https://sourceware.org/git/?p=glibc.git;a=blob;f=elf/dl-usage.c;hb=HEAD#l42
      var regex = new Regex(@"^(?:ldd|ld\.so)\s\(.+\).*\s(?<version>(?:\d+\.){1,3}\d+)\.?\n");
      var grp = regex.Match(output).Groups["version"];
      if (!grp.Success)
        throw new FormatException($"Failed to extract GLibC version from ldd output:\n{output}");
      var versionStr = grp.Captures[0].Value;
      try
      {
        return new Version(versionStr);
      }
      catch (Exception e)
      {
        throw new FormatException($"Failed to parse GLibC version {versionStr} from ldd output:\n{output}", e);
      }
    }

    internal static Version ParseMuslLddOutput(string output)
    {
      // Note(ww898): See https://git.musl-libc.org/cgit/musl/tree/ldso/dynlink.c#n1892
      var regex = new Regex(@"^musl\slibc\s\(.+\)\nVersion\s(?<version>(?:\d+\.){1,3}\d+)(?:_.+)?\n");
      var grp = regex.Match(output).Groups["version"];
      if (!grp.Success)
        throw new FormatException($"Failed to extract MUSL version from ldd output:\n{output}");
      var versionStr = grp.Captures[0].Value;
      try
      {
        return new Version(versionStr);
      }
      catch (Exception e)
      {
        throw new FormatException($"Failed to parse MUSL version {versionStr} from ldd output:\n{output}", e);
      }
    }
  }
}