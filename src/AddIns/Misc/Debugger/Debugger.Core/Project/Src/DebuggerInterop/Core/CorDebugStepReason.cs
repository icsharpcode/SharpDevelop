namespace DebuggerInterop.Core
{
    using System;

    public enum CorDebugStepReason
    {
        // Fields
        STEP_CALL = 2,
        STEP_EXCEPTION_FILTER = 3,
        STEP_EXCEPTION_HANDLER = 4,
        STEP_EXIT = 6,
        STEP_INTERCEPT = 5,
        STEP_NORMAL = 0,
        STEP_RETURN = 1
    }
}

