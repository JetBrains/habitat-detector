#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6)
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  internal static class ERRNO
  {
    #region <errno.h> Declarations

    public const int EPERM = 1;    // Operation not permitted
    public const int ENOENT = 2;   // No such file or directory
    public const int ESRCH = 3;    // No such process
    public const int EINTR = 4;    // Interrupted system call
    public const int EIO = 5;      // Input/output error
    public const int ENXIO = 6;    // Device not configured
    public const int E2BIG = 7;    // Argument list too long
    public const int ENOEXEC = 8;  // Exec format error
    public const int EBADF = 9;    // Bad file descriptor
    public const int ECHILD = 10;  // No child processes
    public const int EDEADLK = 11; // Resource deadlock avoided
    public const int ENOMEM = 12;  // Cannot allocate memory
    public const int EFAULT = 14;  // Bad address
    public const int EBUSY = 16;   // Device / Resource busy
    public const int EEXIST = 17;  // File exists
    public const int EXDEV = 18;   // Cross-device link
    public const int ENODEV = 19;  // Operation not supported by device
    public const int ENOTDIR = 20; // Not a directory
    public const int EISDIR = 21;  // Is a directory
    public const int EINVAL = 22;  // Invalid argument
    public const int ENFILE = 23;  // Too many open files in system
    public const int EMFILE = 24;  // Too many open files
    public const int ENOTTY = 25;  // Inappropriate ioctl for device
    public const int ETXTBSY = 26; // Text file busy
    public const int EFBIG = 27;   // File too large
    public const int ENOSPC = 28;  // No space left on device
    public const int ESPIPE = 29;  // Illegal seek
    public const int EROFS = 30;   // Read-only file system
    public const int EMLINK = 31;  // Too many links
    public const int EPIPE = 32;   // Broken pipe

    /* math software */
    public const int EDOM = 33;   // Numerical argument out of domain
    public const int ERANGE = 34; // Result too large

    /* non-blocking and interrupt i/o */
    public const int EAGAIN = 35;          // Resource temporarily unavailable
    public const int EWOULDBLOCK = EAGAIN; // Operation would block
    public const int EINPROGRESS = 36;     // Operation now in progress
    public const int EALREADY = 37;        // Operation already in progress

    /* ipc/network software -- argument errors */
    public const int ENOTSOCK = 38;        // Socket operation on non-socket
    public const int EDESTADDRREQ = 39;    // Destination address required
    public const int EMSGSIZE = 40;        // Message too long
    public const int EPROTOTYPE = 41;      // Protocol wrong type for socket
    public const int ENOPROTOOPT = 42;     // Protocol not available
    public const int EPROTONOSUPPORT = 43; // Protocol not supported
    public const int ENOTSUP = 45;         // Operation not supported
    public const int EOPNOTSUPP = ENOTSUP; // Operation not supported on socket

    public const int EAFNOSUPPORT = 47;  // Address family not supported by protocol family
    public const int EADDRINUSE = 48;    // Address already in use
    public const int EADDRNOTAVAIL = 49; // Can't assign requested address

    /* ipc/network software -- operational errors */
    public const int ENETDOWN = 50;     // Network is down
    public const int ENETUNREACH = 51;  // Network is unreachable
    public const int ENETRESET = 52;    // Network dropped connection on reset
    public const int ECONNABORTED = 53; // Software caused connection abort
    public const int ECONNRESET = 54;   // Connection reset by peer
    public const int ENOBUFS = 55;      // No buffer space available
    public const int EISCONN = 56;      // Socket is already connected
    public const int ENOTCONN = 57;     // Socket is not connected
    public const int ETIMEDOUT = 60;    // Operation timed out
    public const int ECONNREFUSED = 61; // Connection refused

    public const int ELOOP = 62;        // Too many levels of symbolic links
    public const int ENAMETOOLONG = 63; // File name too long

    /* should be rearranged */
    public const int EHOSTUNREACH = 65; // No route to host
    public const int ENOTEMPTY = 66;    // Directory not empty

    /* quotas & mush */
    public const int EDQUOT = 69; // Disc quota exceeded

    /* Network File System */
    public const int ESTALE = 70; // Stale NFS file handle

    public const int ENOLCK = 77; // No locks available
    public const int ENOSYS = 78; // Function not implemented

    public const int EOVERFLOW = 84; // Value too large to be stored in data type

    public const int ECANCELED = 89; // Operation canceled

    public const int EIDRM = 90;  // Identifier removed
    public const int ENOMSG = 91; // No message of desired type
    public const int EILSEQ = 92; // Illegal byte sequence

    public const int EBADMSG = 94;   // Bad message
    public const int EMULTIHOP = 95; // Reserved
    public const int ENODATA = 96;   // No message available on STREAM
    public const int ENOLINK = 97;   // Reserved
    public const int ENOSR = 98;     // No STREAM resources
    public const int ENOSTR = 99;    // Not a STREAM
    public const int EPROTO = 100;   // Protocol error
    public const int ETIME = 101;    // STREAM ioctl timeout

    public const int ENOTRECOVERABLE = 104; // State not recoverable
    public const int EOWNERDEAD = 105;      // Previous owner died

    #endregion
  }
}
#endif