// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		Dictionary<string, string> environmentVariables = 
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		
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
		
		public Dictionary<string, string> EnvironmentVariables {
			get { return environmentVariables; }
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
