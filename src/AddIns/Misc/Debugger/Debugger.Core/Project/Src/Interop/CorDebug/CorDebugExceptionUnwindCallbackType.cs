// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorDebug
{
    using System;

    public enum CorDebugExceptionUnwindCallbackType
    {
        // Fields
        DEBUG_EXCEPTION_INTERCEPTED = 2,
        DEBUG_EXCEPTION_UNWIND_BEGIN = 1
    }
}
