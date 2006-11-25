// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Wrappers.CorDebug
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

#pragma warning restore 108, 1591 
