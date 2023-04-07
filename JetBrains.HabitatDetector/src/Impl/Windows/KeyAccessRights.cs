using System;
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [Flags]
  internal enum KeyAccessRights : uint
  {
    KEY_QUERY_VALUE = 0x0001,
    KEY_SET_VALUE = 0x0002,
    KEY_CREATE_SUB_KEY = 0x0004,
    KEY_ENUMERATE_SUB_KEYS = 0x0008,
    KEY_NOTIFY = 0x0010,
    KEY_CREATE_LINK = 0x0020,
    KEY_WOW64_32KEY = 0x0200,
    KEY_WOW64_64KEY = 0x0100,
    KEY_WOW64_RES = 0x0300,

    KEY_READ = ~AccessRights.SYNCHRONIZE & (AccessRights.STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY),

    KEY_WRITE = ~AccessRights.SYNCHRONIZE & (AccessRights.STANDARD_RIGHTS_WRITE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY),

    KEY_EXECUTE = ~AccessRights.SYNCHRONIZE & KEY_READ,

    KEY_ALL_ACCESS = ~AccessRights.SYNCHRONIZE & (AccessRights.STANDARD_RIGHTS_ALL | KEY_QUERY_VALUE | KEY_SET_VALUE | KEY_CREATE_SUB_KEY | KEY_ENUMERATE_SUB_KEYS | KEY_NOTIFY | KEY_CREATE_LINK),
  }
}