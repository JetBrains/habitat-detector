using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static class RID
  {
    // Universal well-known
    public const int SECURITY_NULL_RID = 0x00000000;
    public const int SECURITY_WORLD_RID = 0x00000000;
    public const int SECURITY_LOCAL_RID = 0x00000000;
    public const int SECURITY_LOCAL_LOGON_RID = 0x00000001;

    public const int SECURITY_CREATOR_OWNER_RID = 0x00000000;
    public const int SECURITY_CREATOR_GROUP_RID = 0x00000001;

    // NT well-known
    public const int SECURITY_DIALUP_RID = 0x00000001;
    public const int SECURITY_NETWORK_RID = 0x00000002;
    public const int SECURITY_BATCH_RID = 0x00000003;
    public const int SECURITY_INTERACTIVE_RID = 0x00000004;
    public const int SECURITY_LOGON_IDS_RID = 0x00000005;
    public const int SECURITY_SERVICE_RID = 0x00000006;
    public const int SECURITY_ANONYMOUS_LOGON_RID = 0x00000007;
    public const int SECURITY_PROXY_RID = 0x00000008;
    public const int SECURITY_ENTERPRISE_CONTROLLERS_RID = 0x00000009;
    public const int SECURITY_PRINCIPAL_SELF_RID = 0x0000000A;
    public const int SECURITY_AUTHENTICATED_USER_RID = 0x0000000B;
    public const int SECURITY_RESTRICTED_CODE_RID = 0x0000000C;
    public const int SECURITY_TERMINAL_SERVER_RID = 0x0000000D;
    public const int SECURITY_REMOTE_LOGON_RID = 0x0000000E;
    public const int SECURITY_THIS_ORGANIZATION_RID = 0x0000000F;
    public const int SECURITY_IUSER_RID = 0x00000011;
    public const int SECURITY_LOCAL_SYSTEM_RID = 0x00000012;
    public const int SECURITY_LOCAL_SERVICE_RID = 0x00000013;
    public const int SECURITY_NETWORK_SERVICE_RID = 0x00000014;

    public const int SECURITY_BUILTIN_DOMAIN_RID = 0x00000020;

    // well-known aliases ...
    public const int DOMAIN_ALIAS_RID_ADMINS = 0x00000220;
    public const int DOMAIN_ALIAS_RID_USERS = 0x00000221;
    public const int DOMAIN_ALIAS_RID_GUESTS = 0x00000222;
    public const int DOMAIN_ALIAS_RID_POWER_USERS = 0x00000223;

    public const int DOMAIN_ALIAS_RID_ACCOUNT_OPS = 0x00000224;
    public const int DOMAIN_ALIAS_RID_SYSTEM_OPS = 0x00000225;
    public const int DOMAIN_ALIAS_RID_PRINT_OPS = 0x00000226;
    public const int DOMAIN_ALIAS_RID_BACKUP_OPS = 0x00000227;

    public const int DOMAIN_ALIAS_RID_REPLICATOR = 0x00000228;
    public const int DOMAIN_ALIAS_RID_RAS_SERVERS = 0x00000229;
    public const int DOMAIN_ALIAS_RID_PREW2KCOMPACCESS = 0x0000022A;
    public const int DOMAIN_ALIAS_RID_REMOTE_DESKTOP_USERS = 0x0000022B;
    public const int DOMAIN_ALIAS_RID_NETWORK_CONFIGURATION_OPS = 0x0000022C;
    public const int DOMAIN_ALIAS_RID_INCOMING_FOREST_TRUST_BUILDERS = 0x0000022D;

    public const int DOMAIN_ALIAS_RID_MONITORING_USERS = 0x0000022E;
    public const int DOMAIN_ALIAS_RID_LOGGING_USERS = 0x0000022F;
    public const int DOMAIN_ALIAS_RID_AUTHORIZATIONACCESS = 0x00000230;
    public const int DOMAIN_ALIAS_RID_TS_LICENSE_SERVERS = 0x00000231;
    public const int DOMAIN_ALIAS_RID_DCOM_USERS = 0x00000232;
    public const int DOMAIN_ALIAS_RID_IUSERS = 0x00000238;
    public const int DOMAIN_ALIAS_RID_CRYPTO_OPERATORS = 0x00000239;
    public const int DOMAIN_ALIAS_RID_CACHEABLE_PRINCIPALS_GROUP = 0x0000023B;
    public const int DOMAIN_ALIAS_RID_NON_CACHEABLE_PRINCIPALS_GROUP = 0x0000023C;
    public const int DOMAIN_ALIAS_RID_EVENT_LOG_READERS_GROUP = 0x0000023D;
    public const int DOMAIN_ALIAS_RID_CERTSVC_DCOM_ACCESS_GROUP = 0x0000023E;
    public const int DOMAIN_ALIAS_RID_RDS_REMOTE_ACCESS_SERVERS = 0x0000023F;
    public const int DOMAIN_ALIAS_RID_RDS_ENDPOINT_SERVERS = 0x00000240;
    public const int DOMAIN_ALIAS_RID_RDS_MANAGEMENT_SERVERS = 0x00000241;
    public const int DOMAIN_ALIAS_RID_HYPER_V_ADMINS = 0x00000242;
    public const int DOMAIN_ALIAS_RID_ACCESS_CONTROL_ASSISTANCE_OPS = 0x00000243;
    public const int DOMAIN_ALIAS_RID_REMOTE_MANAGEMENT_USERS = 0x00000244;
    public const int DOMAIN_ALIAS_RID_DEFAULT_ACCOUNT = 0x00000245;
    public const int DOMAIN_ALIAS_RID_STORAGE_REPLICA_ADMINS = 0x00000246;
    public const int DOMAIN_ALIAS_RID_DEVICE_OWNERS = 0x00000247;

    // Application Package Authority
    public const int SECURITY_APP_PACKAGE_BASE_RID = 0x00000002;
    public const int SECURITY_CAPABILITY_BASE_RID = 0x00000003;
    public const int SECURITY_CAPABILITY_APP_RID = 0x000000400;

    // Built-in Packages
    public const int SECURITY_BUILTIN_PACKAGE_ANY_PACKAGE = 0x00000001;
    public const int SECURITY_BUILTIN_PACKAGE_ANY_RESTRICTED_PACKAGE = 0x00000002;

    // Mandatory Label Authority
    public const int SECURITY_MANDATORY_UNTRUSTED_RID = 0x00000000;
    public const int SECURITY_MANDATORY_LOW_RID = 0x00001000;
    public const int SECURITY_MANDATORY_MEDIUM_RID = 0x00002000;
    public const int SECURITY_MANDATORY_MEDIUM_PLUS_RID = SECURITY_MANDATORY_MEDIUM_RID + 0x100;
    public const int SECURITY_MANDATORY_HIGH_RID = 0x00003000;
    public const int SECURITY_MANDATORY_SYSTEM_RID = 0x00004000;
    public const int SECURITY_MANDATORY_PROTECTED_PROCESS_RID = 0x00005000;
  }
}