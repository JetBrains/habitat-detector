using System;
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [Flags]
  internal enum ProcessAccessRights : uint
  {
    PROCESS_TERMINATE                 = 0x0001,   // Required to terminate a process using TerminateProcess.
    PROCESS_CREATE_THREAD             = 0x0002,   // Required to create a thread.
    PROCESS_SET_SESSIONID             = 0x0004,
    PROCESS_VM_OPERATION              = 0x0008,   // Required to perform an operation on the address space of a process (see VirtualProtectEx and WriteProcessMemory).
    PROCESS_VM_READ                   = 0x0010,   // Required to read memory in a process using ReadProcessMemory.
    PROCESS_VM_WRITE                  = 0x0020,   // Required to write to memory in a process using WriteProcessMemory.
    PROCESS_DUP_HANDLE                = 0x0040,   // Required to duplicate a handle using DuplicateHandle.
    PROCESS_CREATE_PROCESS            = 0x0080,   // Required to create a process.
    PROCESS_SET_QUOTA                 = 0x0100,   // Required to set memory limits using SetProcessWorkingSetSize.
    PROCESS_SET_INFORMATION           = 0x0200,   // Required to set certain information about a process, such as its priority class (see SetPriorityClass).
    PROCESS_QUERY_INFORMATION         = 0x0400,   // Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken).
    PROCESS_SUSPEND_RESUME            = 0x0800,   // Required to suspend or resume a process.
    PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,   // Required to retrieve certain information about a process (see GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName). A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.

    PROCESS_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFFF,

    // The following are masks for the predefined standard access types.
    DELETE                            = 0x00010000, // Required to delete the object.
    READ_CONTROL                      = 0x00020000, // Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right.
    WRITE_DAC                         = 0x00040000, // Required to modify the DACL in the security descriptor for the object.
    WRITE_OWNER                       = 0x00080000, // Required to change the owner in the security descriptor for the object.
    SYNCHRONIZE                       = 0x00100000, // The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.
    STANDARD_RIGHTS_REQUIRED          = 0x000F0000,
    STANDARD_RIGHTS_READ              = READ_CONTROL,
    STANDARD_RIGHTS_WRITE             = READ_CONTROL,
    STANDARD_RIGHTS_EXECUTE           = READ_CONTROL,
    STANDARD_RIGHTS_ALL               = 0x001F0000,
    SPECIFIC_RIGHTS_ALL               = 0x0000FFFF,

    ACCESS_SYSTEM_SECURITY            = 0x01000000
  }
}