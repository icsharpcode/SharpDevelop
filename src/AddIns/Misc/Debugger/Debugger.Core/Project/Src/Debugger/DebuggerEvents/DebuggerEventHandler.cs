// <file>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
// </file>

using System;

namespace DebuggerLibrary 
{	
	public delegate void DebuggerEventHandler (object sender, DebuggerEventArgs e);
	
	public class DebuggerEventArgs : System.EventArgs 
	{
		// Some parameters are expected in the furture
	}
}
