namespace DebuggerInterop.Core
{
    using System;

    public enum CorDebugInternalFrameType
    {
        // Fields
        STUBFRAME_APPDOMAIN_TRANSITION = 3,
        STUBFRAME_FUNC_EVAL = 5,
        STUBFRAME_INTERNALCALL = 6,
        STUBFRAME_LIGHTWEIGHT_FUNCTION = 4,
        STUBFRAME_M2U = 1,
        STUBFRAME_NONE = 0,
        STUBFRAME_U2M = 2
    }
}

