// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestProcessRunner : IUnitTestProcessRunner
	{
		ProcessRunner runner;
		
		public event LineReceivedEventHandler OutputLineReceived {
			add { runner.OutputLineReceived += value; }
			remove { runner.OutputLineReceived -= value; }
		}
		
		public event LineReceivedEventHandler ErrorLineReceived {
			add { runner.ErrorLineReceived += value; }
			remove { runner.ErrorLineReceived -= value; }
		}
		
		public event EventHandler ProcessExited {
			add { runner.ProcessExited += value; }
			remove { runner.ProcessExited -= value; }
		}
		
		public UnitTestProcessRunner()
		{
			runner = new ProcessRunner();
		}
		
		public bool LogStandardOutputAndError {
			get { return runner.LogStandardOutputAndError; }
			set { runner.LogStandardOutputAndError = value; }
		}
		
		public string WorkingDirectory {
			get { return runner.WorkingDirectory; }
			set { runner.WorkingDirectory = value; }
		}
		
		public Dictionary<string, string> EnvironmentVariables {
			get { return runner.EnvironmentVariables; }
		}
		
		public void Start(string command, string arguments)
		{
			runner.Start(command, arguments);
		}
		
		public void Kill()
		{
			runner.Kill();
		}
	}
}
