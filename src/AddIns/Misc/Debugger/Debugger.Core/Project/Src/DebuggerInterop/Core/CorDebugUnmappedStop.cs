namespace DebuggerInterop.Core
{
    using System;

    public enum CorDebugUnmappedStop
    {
        // Fields
        STOP_ALL = 0xffff,
        STOP_EPILOG = 2,
        STOP_NO_MAPPING_INFO = 4,
        STOP_NONE = 0,
        STOP_OTHER_UNMAPPED = 8,
        STOP_PROLOG = 1,
        STOP_UNMANAGED = 0x10
    }
}

