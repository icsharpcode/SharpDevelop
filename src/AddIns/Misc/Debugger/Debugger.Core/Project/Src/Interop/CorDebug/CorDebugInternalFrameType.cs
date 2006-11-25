// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
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

#pragma warning restore 108, 1591