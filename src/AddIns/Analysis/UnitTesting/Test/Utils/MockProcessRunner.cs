// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockProcessRunner : IUnitTestProcessRunner
	{
		bool logStandardOutputAndError = true;
		string workingDirectory;
		string commandPassedToStartMethod;
		string commandArgumentsPassedToStartMethod;
		bool killMethodCalled;
		
		public event EventHandler ProcessExited;
		public event LineReceivedEventHandler OutputLineReceived;
		public event LineReceivedEventHandler ErrorLineReceived;
		
		public bool LogStandardOutputAndError {
			get { return logStandardOutputAndError; }
			set { logStandardOutputAndError = value; }
		}
		
		public string WorkingDirectory {
			get { return workingDirectory; }
			set { workingDirectory = value; }
		}
		
		public void Start(string command, string arguments)
		{
			commandPassedToStartMethod = command;
			commandArgumentsPassedToStartMethod = arguments;
		}
		
		public string CommandPassedToStartMethod {
			get { return commandPassedToStartMethod; }
		}
		
		public string CommandArgumentsPassedToStartMethod {
			get { return commandArgumentsPassedToStartMethod; }
		}
		
		public void Kill()
		{
			killMethodCalled = true;
		}
		
		public bool IsKillMethodCalled {
			get { return killMethodCalled; }
			set { killMethodCalled = value; }
		}
		
		public void FireProcessExitedEvent()
		{
			if (ProcessExited != null) {
				ProcessExited(this, new EventArgs());
			}
		}
		
		public void FireOutputLineReceivedEvent(LineReceivedEventArgs e)
		{
			if (OutputLineReceived != null) {
				OutputLineReceived(this, e);
			}
		}
		
		public void FireErrorLineReceivedEvent(LineReceivedEventArgs e)
		{
			if (ErrorLineReceived != null) {
				ErrorLineReceived(this, e);
			}
		}
	}
}
