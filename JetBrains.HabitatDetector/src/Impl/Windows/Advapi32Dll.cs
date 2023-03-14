using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal static unsafe class Advapi32Dll
  {
    internal const string LibraryName = "advapi32.dll";

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int RegOpenKeyExW(void* hKey, string lpSubKey, uint ulOptions, uint samDesired, void** phkResult);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int RegQueryValueExW(void* hKey, string lpValueName, uint* lpReserved, REG* lpType, byte* lpData, uint* lpcbData);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, ExactSpelling = true)]
    internal static extern int RegCloseKey(void* hKey);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int OpenProcessToken(void* ProcessHandle, uint DesiredAccess, void** TokenHandle);

    [DllImport(LibraryName, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
    internal static extern int GetTokenInformation(void* TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, void* TokenInformation, uint TokenInformationLength, uint* ReturnLength);

    [DllImport(LibraryName, ExactSpelling = true, SetLastError = true)]
    internal static extern int AllocateAndInitializeSid(
      SID_IDENTIFIER_AUTHORITY* pIdentifierAuthority, byte nSubAuthorityCount,
      uint dwSubAuthority0, uint dwSubAuthority1, uint dwSubAuthority2, uint dwSubAuthority3,
      uint dwSubAuthority4, uint dwSubAuthority5, uint dwSubAuthority6, uint dwSubAuthority7,
      void** pSid);

    [DllImport(LibraryName, ExactSpelling = true)]
    internal static extern void* FreeSid(void* pSid);

    [DllImport(LibraryName, SetLastError = true, ExactSpelling = true)]
    internal static extern int DuplicateToken(void* ExistingTokenHandle, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, void** DuplicateTokenHandle);

    [DllImport(LibraryName, SetLastError = true, ExactSpelling = true)]
    internal static extern int CheckTokenMembership(void* TokenHandle, void* SidToCheck, int* IsMember);
  }
}