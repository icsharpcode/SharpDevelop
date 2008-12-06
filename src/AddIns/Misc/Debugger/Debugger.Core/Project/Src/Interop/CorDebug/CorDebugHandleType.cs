// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
{
    public enum CorDebugHandleType
    {
        // Fields
        HANDLE_STRONG = 1,
        HANDLE_WEAK_TRACK_RESURRECTION = 2
    }
}

#pragma warning restore 108, 1591