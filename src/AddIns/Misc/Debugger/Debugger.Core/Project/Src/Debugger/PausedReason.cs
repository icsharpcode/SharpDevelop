// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger
{
	public enum PausedReason : int
	{
		EvalComplete,
		StepComplete,
		Breakpoint,
		Break,
		ControlCTrap,
		Exception,
		ForcedBreak, // Process.Break called
		DebuggerError,
		CurrentThreadChanged,
		CurrentFunctionChanged,
		ExceptionIntercepted,
		SetIP,
		Other
	}
}
