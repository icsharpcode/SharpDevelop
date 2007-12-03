// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Process runner interface.
	/// </summary>
	public interface IProcessRunner
	{
		/// <summary>
		/// Triggered when a line of text is read from the standard output.
		/// </summary>		
		event LineReceivedEventHandler OutputLineReceived;
		
		/// <summary>
		/// Triggered when the process has exited.
		/// </summary>
		event EventHandler ProcessExited;
		
		/// <summary>
		/// Starts the process.
		/// </summary>
		/// <param name="command">The process filename.</param>
		/// <param name="arguments">The command line arguments to
		/// pass to the command.</param>
		void Start(string command, string arguments);
		
		/// <summary>
		/// Kills the running process.
		/// </summary>
		void Kill();
	}
}
