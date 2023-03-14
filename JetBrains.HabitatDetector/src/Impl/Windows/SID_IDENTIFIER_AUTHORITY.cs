using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [StructLayout(LayoutKind.Sequential)]
  internal unsafe struct SID_IDENTIFIER_AUTHORITY
  {
    internal fixed byte Value[6];

    // @formatter:off
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_NULL_SID_AUTHORITY         = new(0, 0, 0, 0, 0,  0);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_WORLD_SID_AUTHORITY        = new(0, 0, 0, 0, 0,  1);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_LOCAL_SID_AUTHORITY        = new(0, 0, 0, 0, 0,  2);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_CREATOR_SID_AUTHORITY      = new(0, 0, 0, 0, 0,  3);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_NON_UNIQUE_AUTHORITY       = new(0, 0, 0, 0, 0,  4);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_NT_AUTHORITY               = new(0, 0, 0, 0, 0,  5);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_RESOURCE_MANAGER_AUTHORITY = new(0, 0, 0, 0, 0,  9);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_APP_PACKAGE_AUTHORITY      = new(0, 0, 0, 0, 0, 15);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_MANDATORY_LABEL_AUTHORITY  = new(0, 0, 0, 0, 0, 16);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_SCOPED_POLICY_ID_AUTHORITY = new(0, 0, 0, 0, 0, 17);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_AUTHENTICATION_AUTHORITY   = new(0, 0, 0, 0, 0, 18);
    internal static readonly SID_IDENTIFIER_AUTHORITY SECURITY_PROCESS_TRUST_AUTHORITY    = new(0, 0, 0, 0, 0, 19);
    // @formatter:on

    private SID_IDENTIFIER_AUTHORITY(byte value0, byte value1, byte value2, byte value3, byte value4, byte value5)
    {
      Value[0] = value0;
      Value[1] = value1;
      Value[2] = value2;
      Value[3] = value3;
      Value[4] = value4;
      Value[5] = value5;
    }
  }
}