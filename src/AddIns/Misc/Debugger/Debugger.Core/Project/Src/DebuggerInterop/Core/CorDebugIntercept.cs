namespace DebuggerInterop.Core
{
    using System;

    public enum CorDebugIntercept
    {
        // Fields
        INTERCEPT_ALL = 0xffff,
        INTERCEPT_CLASS_INIT = 1,
        INTERCEPT_CONTEXT_POLICY = 8,
        INTERCEPT_EXCEPTION_FILTER = 2,
        INTERCEPT_INTERCEPTION = 0x10,
        INTERCEPT_NONE = 0,
        INTERCEPT_SECURITY = 4
    }
}

