// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
