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

    public enum CorDebugUserState
    {
        // Fields
        USER_BACKGROUND = 4,
        USER_STOP_REQUESTED = 1,
        USER_STOPPED = 0x10,
        USER_SUSPEND_REQUESTED = 2,
        USER_SUSPENDED = 0x40,
        USER_UNSAFE_POINT = 0x80,
        USER_UNSTARTED = 8,
        USER_WAIT_SLEEP_JOIN = 0x20
    }
}

#pragma warning restore 108, 1591