using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  // See https://social.technet.microsoft.com/Forums/windows/en-US/bf693b49-1dd5-45ee-84cf-4a417e5b35ec/run-as-admin-event-log?forum=winserverDS
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal enum TOKEN_ELEVATION_TYPE
  {
    /// <summary>
    ///   Type 1 is a full token with no privileges removed or groups disabled. A full token is only used if User Account
    ///   Control is disabled or if the user is the built-in Administrator account (for which UAC disabled by default), service
    ///   account or local system account.
    /// </summary>
    TokenElevationTypeDefault = 1,

    /// <summary>
    ///   Type 2 is an elevated token with no privileges removed or groups disabled. An elevated token is used when User
    ///   Account Control is enabled and the user chooses to start the program using Run as administrator. An elevated token is
    ///   also used when an application is configured to always require administrative privilege or to always require maximum
    ///   privilege, and the user is a member of the Administrators group.
    /// </summary>
    TokenElevationTypeFull,

    /// <summary>
    ///   Type 3 is a limited token with administrative privileges removed and administrative groups disabled. The limited
    ///   token is used when User Account Control is enabled, the application does not require administrative privilege, and
    ///   the user does not choose to start the program using Run as administrator.
    /// </summary>
    TokenElevationTypeLimited
  }
}