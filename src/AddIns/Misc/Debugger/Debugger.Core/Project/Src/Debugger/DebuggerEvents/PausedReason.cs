// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger
{
	public enum PausedReason:int
	{
		AllEvalsComplete,
		StepComplete,
		Breakpoint,
		Break,
		ControlCTrap,
		Exception,
		DebuggerError,
		EvalComplete,
		CurrentThreadChanged,
		CurrentFunctionChanged,
		ExceptionIntercepted,
		SetIP
	}
}
