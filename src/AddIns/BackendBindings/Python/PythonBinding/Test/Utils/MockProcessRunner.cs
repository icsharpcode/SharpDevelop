// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Util;

namespace PythonBinding.Tests.Utils
{
	/// <summary>
	/// Dummy IProcessRunner class.
	/// </summary>
	public class MockProcessRunner : IProcessRunner
	{
		string commandLine = String.Empty;
		string outputText;
		bool killCalled;
		
		/// <summary>
		/// Triggered when a line of text is read from the standard output.
		/// </summary>		
		public event LineReceivedEventHandler OutputLineReceived;
		
		/// <summary>
		/// Triggered when the process has exited.
		/// </summary>
		public event EventHandler ProcessExited;
		
		public MockProcessRunner()
		{
		}
		
		/// <summary>
		/// Starts the process.
		/// </summary>
		/// <param name="command">The process filename.</param>
		/// <param name="arguments">The command line arguments to
		/// pass to the command.</param>
		public void Start(string command, string arguments)
		{
			commandLine = String.Concat(command, " ", arguments);
			if (outputText != null) {
				OnOutputLineReceived();
			}
		}
		
		/// <summary>
		/// Kills the running process.
		/// </summary>
		public void Kill()
		{
			killCalled = true;
		}
		
		/// <summary>
		/// Gets the full command line used with the process runner.
		/// </summary>
		public string CommandLine {
			get {
				return commandLine;
			}
		}
		
		/// <summary>
		/// The string will be sent to any OutputLineReceived event
		/// handler when the Start method is called.
		/// </summary>
		public string OutputText {
			set {
				outputText = value;
			}
		}
		
		/// <summary>
		/// Raises the ProcessExited event.
		/// </summary>
		public void RaiseExitEvent()
		{
			OnProcessExited(new EventArgs());
		}
		
		/// <summary>
		/// Indicates whether the Kill method was called.
		/// </summary>
		public bool KillCalled {
			get {
				return killCalled;
			}
		}
		
		/// <summary>
		/// Raises the ProcessExited event.
		/// </summary>
		void OnProcessExited(EventArgs e)
		{
			if (ProcessExited != null) {
				ProcessExited(this, e);
			}
		}	
		
		/// <summary>
		/// Raises the OutputLineReceived event.
		/// </summary>
		void OnOutputLineReceived(LineReceivedEventArgs e)
		{
			if (OutputLineReceived != null) {
				OutputLineReceived(this, e);
			}
		}	
		
		/// <summary>
		/// Raises an event for each line in the output text.
		/// </summary>
		void OnOutputLineReceived()
		{
			using (StringReader reader = new StringReader(outputText)) {
				string line;
				do {
					line = reader.ReadLine();
					if (line != null) {
						OnOutputLineReceived(new LineReceivedEventArgs(line));
					}
				} while (line != null);
			}
		}
	}
}
