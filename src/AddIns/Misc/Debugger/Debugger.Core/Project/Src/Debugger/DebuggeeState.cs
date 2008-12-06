// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>


namespace Debugger
{
	/// <summary>
	/// Represents span of time in which the debugger state is assumed to
	/// be unchanged.
	/// </summary>
	/// <remarks>
	/// For example, although property evaluation can in theory change
	/// any memory, it is assumed that they behave 'correctly' and thus
	/// property evaluation does not change debugger state.
	/// </remarks>
	public class DebuggeeState: DebuggerObject
	{
		Process process;
		
		[Tests.Ignore]
		public Process Process {
			get {
				return process;
			}
		}
		
		public DebuggeeState(Process process)
		{
			this.process = process;
		}
	}
}
