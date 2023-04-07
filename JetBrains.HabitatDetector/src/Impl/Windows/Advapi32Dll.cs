using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static unsafe class Advapi32Dll
  {
    internal const string LibraryName = "advapi32.dll";

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int RegOpenKeyExW(void* hKey, string lpSubKey, uint ulOptions, uint samDesired, void** phkResult);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int RegQueryValueExW(void* hKey, string lpValueName, uint* lpReserved, REG* lpType, byte* lpData, uint* lpcbData);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int RegCloseKey(void* hKey);
  }
}