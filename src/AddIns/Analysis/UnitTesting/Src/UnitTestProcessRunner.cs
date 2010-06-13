// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
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
