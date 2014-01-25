// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
