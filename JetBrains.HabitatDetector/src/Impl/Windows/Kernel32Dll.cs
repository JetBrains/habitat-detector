using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static unsafe class Kernel32Dll
  {
    internal const string LibraryName = "kernel32.dll";

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern void* GetCurrentProcess();

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern void* GetModuleHandleW(string lpModuleName);

    [DllImport(LibraryName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    internal static extern void* GetProcAddress(void* hModule, string lpProcName);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern void GetSystemInfo(SYSTEM_INFO* lpSystemInfo);

    // | OS    | Process | wProcessorArchitecture           |
    // +=======+=========+==================================+
    // | ARM64 | ARM64   | 12(PROCESSOR_ARCHITECTURE_ARM64) |
    // | ARM64 | ARM     | 12(PROCESSOR_ARCHITECTURE_ARM64) |
    // | ARM64 | X64     |  9(PROCESSOR_ARCHITECTURE_AMD64) | ATTENTION: Returns 9 for compatibility !!!
    // | ARM64 | X86     |  9(PROCESSOR_ARCHITECTURE_AMD64) | ATTENTION: Returns 9 for compatibility !!!
    // | X64   | X64     |  9(PROCESSOR_ARCHITECTURE_AMD64) |
    // | X64   | X86     |  9(PROCESSOR_ARCHITECTURE_AMD64) |
    // | X86   | X86     |  0(PROCESSOR_ARCHITECTURE_INTEL) |
    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern void GetNativeSystemInfo(SYSTEM_INFO* lpSystemInfo);

    // | OS    | Process | Wow64Process |
    // +=======+=========+==============+
    // | ARM64 | ARM64   | 0(FALSE)     |
    // | ARM64 | ARM     | 1(TRUE )     |
    // | ARM64 | X64     | 0(FALSE)     |
    // | ARM64 | X86     | 1(TRUE )     |
    // | X64   | X64     | 0(FALSE)     |
    // | X64   | X86     | 1(TRUE )     |
    // | X86   | X86     | 0(FALSE)     |
    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int IsWow64Process(void* hProcess, int* Wow64Process);

    // | OS    | Process | pProcessMachine                    | pNativeMachine                   |
    // +=======+=========+====================================+==================================+
    // | ARM64 | ARM64   | 0x0000(IMAGE_FILE_MACHINE_UNKNOWN) | 0xAA64(IMAGE_FILE_MACHINE_ARM64) |
    // | ARM64 | ARM     | 0x01C4(IMAGE_FILE_MACHINE_ARMNT  ) | 0xAA64(IMAGE_FILE_MACHINE_ARM64) |
    // | ARM64 | X64     | 0x0000(IMAGE_FILE_MACHINE_UNKNOWN) | 0xAA64(IMAGE_FILE_MACHINE_ARM64) | ATTENTION: Returns 0xAA64 !!!
    // | ARM64 | X86     | 0x014C(IMAGE_FILE_MACHINE_I386   ) | 0xAA64(IMAGE_FILE_MACHINE_ARM64) |
    // | X64   | X64     | 0x0000(IMAGE_FILE_MACHINE_UNKNOWN) | 0x8664(IMAGE_FILE_MACHINE_AMD64) |
    // | X64   | X86     | 0x014C(IMAGE_FILE_MACHINE_I386   ) | 0x8664(IMAGE_FILE_MACHINE_AMD64) |
    // | X86   | X86     | 0x0000(IMAGE_FILE_MACHINE_UNKNOWN) | 0x014C(IMAGE_FILE_MACHINE_I386 ) |
    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int IsWow64Process2(void* hProcess, IMAGE_FILE_MACHINE* pProcessMachine, IMAGE_FILE_MACHINE* pNativeMachine);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int GetProcessInformation(void* hProcess, PROCESS_INFORMATION_CLASS ProcessInformationClass, void* ProcessInformation, uint ProcessInformationSize);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern void* OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int CloseHandle(void* hObject);

    internal delegate int IsWow64ProcessDelegate(void* hProcess, int* Wow64Process);

    internal delegate int IsWow64Process2Delegate(void* hProcess, IMAGE_FILE_MACHINE* pProcessMachine, IMAGE_FILE_MACHINE* pNativeMachine);

    internal delegate int GetProcessInformationDelegate(void* hProcess, PROCESS_INFORMATION_CLASS ProcessInformationClass, void* ProcessInformation, uint ProcessInformationSize);
  }
}