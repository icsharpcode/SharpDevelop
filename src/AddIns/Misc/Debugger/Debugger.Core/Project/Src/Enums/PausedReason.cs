// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

namespace DebuggerLibrary
{
	public enum PausedReason:int
	{
		StepComplete,
		Breakpoint,
		Break,
		ControlCTrap,
		Exception,
		DebuggerError,
		CurrentThreadChanged
	}
}
