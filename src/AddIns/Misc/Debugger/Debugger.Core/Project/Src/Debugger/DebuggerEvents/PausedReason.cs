// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
