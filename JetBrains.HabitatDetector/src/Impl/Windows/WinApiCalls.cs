using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  internal static class WinApiCalls
  {
    public static readonly Kernel32Dll.IsWow64ProcessDelegate? IsWow64Process;
    public static readonly Kernel32Dll.IsWow64Process2Delegate? IsWow64Process2;
    public static readonly Kernel32Dll.GetProcessInformationDelegate? GetProcessInformation;

    static unsafe WinApiCalls()
    {
      var hKernel32Dll = Kernel32Dll.GetModuleHandleW(Kernel32Dll.LibraryName);
      if (hKernel32Dll == null)
        throw new Win32Exception();

      var pIsWow64Process = Kernel32Dll.GetProcAddress(hKernel32Dll, nameof(Kernel32Dll.IsWow64Process));
      if (pIsWow64Process != null)
#pragma warning disable CS0618
        IsWow64Process = (Kernel32Dll.IsWow64ProcessDelegate)Marshal.GetDelegateForFunctionPointer((IntPtr)pIsWow64Process, typeof(Kernel32Dll.IsWow64ProcessDelegate));
#pragma warning restore CS0618

      var pIsWow64Process2 = Kernel32Dll.GetProcAddress(hKernel32Dll, nameof(Kernel32Dll.IsWow64Process2));
      if (pIsWow64Process2 != null)
#pragma warning disable CS0618
        IsWow64Process2 = (Kernel32Dll.IsWow64Process2Delegate)Marshal.GetDelegateForFunctionPointer((IntPtr)pIsWow64Process2, typeof(Kernel32Dll.IsWow64Process2Delegate));
#pragma warning restore CS0618

      var pGetProcessInformation = Kernel32Dll.GetProcAddress(hKernel32Dll, nameof(Kernel32Dll.GetProcessInformation));
      if (pGetProcessInformation != null)
#pragma warning disable CS0618
        GetProcessInformation = (Kernel32Dll.GetProcessInformationDelegate)Marshal.GetDelegateForFunctionPointer((IntPtr)pGetProcessInformation, typeof(Kernel32Dll.GetProcessInformationDelegate));
#pragma warning restore CS0618
    }
  }
}