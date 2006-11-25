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

    public enum CorDebugThreadState
    {
        // Fields
        THREAD_RUN = 0,
        THREAD_SUSPEND = 1
    }
}

#pragma warning restore 108, 1591