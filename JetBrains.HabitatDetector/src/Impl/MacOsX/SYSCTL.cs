using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.MacOsX
{
  [SuppressMessage("ReSharper", "IdentifierTypo")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static class SYSCTL
  {
    internal const int CTL_KERN    = 1; // "high kernel": proc, limits
    internal const int CTL_VM      = 2; // virtual memory
    internal const int CTL_VFS     = 3; // file system, mount type is next
    internal const int CTL_NET     = 4; // network, see socket.h
    internal const int CTL_DEBUG   = 5; // debugging parameters
    internal const int CTL_HW      = 6; // generic cpu/io
    internal const int CTL_MACHDEP = 7; // machine dependent
    internal const int CTL_USER    = 8; // user-level

    internal const int KERN_OSTYPE    =  1; // string: system version
    internal const int KERN_OSRELEASE =  2; // string: system release
    internal const int KERN_OSREV     =  3; // int: system revision
    internal const int KERN_VERSION   =  4; // string: compile time info
    internal const int KERN_MAXVNODES =  5; // int: max vnodes
    internal const int KERN_MAXPROC   =  6; // int: max processes
    internal const int KERN_MAXFILES  =  7; // int: max open files
    internal const int KERN_ARGMAX    =  8; // int: max arguments to exec
    internal const int KERN_PROC      = 14; // struct: process entries
    internal const int KERN_PROCARGS  = 38;
    internal const int KERN_PROCARGS2 = 49;
    internal const int KERN_OSVERSION = 65;

    internal const int KERN_PROC_ALL     = 0; // everything
    internal const int KERN_PROC_PID     = 1; // by process id
    internal const int KERN_PROC_PGRP    = 2; // by process group id
    internal const int KERN_PROC_SESSION = 3; // by session of pid
    internal const int KERN_PROC_TTY     = 4; // by controlling tty
    internal const int KERN_PROC_UID     = 5; // by effective uid
    internal const int KERN_PROC_RUID    = 6; // by real uid
    internal const int KERN_PROC_LCID    = 7; // by login context id

    internal const int HW_MEMSIZE = 24; // ulong: physical ram size
  }
}