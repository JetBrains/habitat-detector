using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static unsafe class HKEY
  {
    // @formatter:off
    internal static readonly void* HKEY_CLASSES_ROOT                = (void*)0x80000000;
    internal static readonly void* HKEY_CURRENT_USER                = (void*)0x80000001;
    internal static readonly void* HKEY_LOCAL_MACHINE               = (void*)0x80000002;
    internal static readonly void* HKEY_USERS                       = (void*)0x80000003;
    internal static readonly void* HKEY_PERFORMANCE_DATA            = (void*)0x80000004;
    internal static readonly void* HKEY_PERFORMANCE_TEXT            = (void*)0x80000050;
    internal static readonly void* HKEY_PERFORMANCE_NLSTEXT         = (void*)0x80000060;
    internal static readonly void* HKEY_CURRENT_CONFIG              = (void*)0x80000005;
    internal static readonly void* HKEY_DYN_DATA                    = (void*)0x80000006;
    internal static readonly void* HKEY_CURRENT_USER_LOCAL_SETTINGS = (void*)0x80000007;
    // @formatter:on
  }
}