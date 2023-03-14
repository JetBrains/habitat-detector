using System;
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [Flags]
  internal enum TokenSpecificAccessRights : uint
  {
    TOKEN_ASSIGN_PRIMARY = 0x0001,
    TOKEN_DUPLICATE = 0x0002,
    TOKEN_IMPERSONATE = 0x0004,
    TOKEN_QUERY = 0x0008,
    TOKEN_QUERY_SOURCE = 0x0010,
    TOKEN_ADJUST_PRIVILEGES = 0x0020,
    TOKEN_ADJUST_GROUPS = 0x0040,
    TOKEN_ADJUST_DEFAULT = 0x0080,
    TOKEN_ADJUST_SESSIONID = 0x0100,

    TOKEN_ALL_ACCESS_P = AccessRights.STANDARD_RIGHTS_REQUIRED |
                         TOKEN_ASSIGN_PRIMARY |
                         TOKEN_DUPLICATE |
                         TOKEN_IMPERSONATE |
                         TOKEN_QUERY |
                         TOKEN_QUERY_SOURCE |
                         TOKEN_ADJUST_PRIVILEGES |
                         TOKEN_ADJUST_GROUPS |
                         TOKEN_ADJUST_DEFAULT,

    TOKEN_ALL_ACCESS = TOKEN_ALL_ACCESS_P |
                       TOKEN_ADJUST_SESSIONID,

    TOKEN_READ = AccessRights.STANDARD_RIGHTS_READ |
                 TOKEN_QUERY,

    TOKEN_WRITE = AccessRights.STANDARD_RIGHTS_WRITE |
                  TOKEN_ADJUST_PRIVILEGES |
                  TOKEN_ADJUST_GROUPS |
                  TOKEN_ADJUST_DEFAULT,

    TOKEN_EXECUTE = AccessRights.STANDARD_RIGHTS_EXECUTE,
  }
}