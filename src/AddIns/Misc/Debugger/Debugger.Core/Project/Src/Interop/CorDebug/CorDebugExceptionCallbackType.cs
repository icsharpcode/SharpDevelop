// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Wrappers.CorDebug
{
    public enum CorDebugExceptionCallbackType
    {
        // Fields
        DEBUG_EXCEPTION_CATCH_HANDLER_FOUND = 3,
        DEBUG_EXCEPTION_FIRST_CHANCE = 1,
        DEBUG_EXCEPTION_UNHANDLED = 4,
        DEBUG_EXCEPTION_USER_FIRST_CHANCE = 2
    }
}

#pragma warning restore 108, 1591
