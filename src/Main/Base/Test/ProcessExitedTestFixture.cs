// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading;

using ICSharpCode.SharpDevelop.Util;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class ProcessExitedTestFixture : ConsoleAppTestFixtureBase
	{		
		/// <summary>
		/// Stores standard output received by the ProcessExit event.
		/// </summary>
		string standardOutput;
		
		/// <summary>
		/// Stores standard error received by the ProcessExit event.
		/// </summary>
		string standardError;
		
		/// <summary>
		/// Stores exit code received by the ProcessExit event.
		/// </summary>		
		int exitCode = -1;
		
		/// <summary>
		/// Event that will be fired when the ProcessExit event occurs.
		/// </summary>
		AutoResetEvent exitEvent;

		/// <summary>
		/// Tests the Runner.ProcessExit event works.
		/// </summary>
		[Test]
		public void ProcessExitEvent()
		{			
			exitEvent = new AutoResetEvent(false);
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			string echoText = "Test";
			string expectedOutput = String.Concat(echoText, "\r\n");
			
			runner.ProcessExited += new EventHandler(OnProcessExited);
			
			runner.Start(GetConsoleAppFileName(), String.Concat("-echo:", echoText));
			bool exited = exitEvent.WaitOne(2500, true);
			
			Assert.IsTrue(exited, "Timed out waiting for exit event.");
			Assert.AreEqual(0, exitCode, "Exit code should be zero.");
			Assert.AreEqual(expectedOutput, standardOutput, "Should have some output.");
			Assert.AreEqual(String.Empty, standardError, "Should not be any error output.");			
		}
		
		/// <summary>
		/// Handles the ProcessExited event.
		/// </summary>
		void OnProcessExited(object sender, EventArgs e)
		{
			ProcessRunner runner = (ProcessRunner)sender;
			
			exitCode = runner.ExitCode;
			standardOutput = runner.StandardOutput;
			standardError = runner.StandardError;
			
			exitEvent.Set();
		}
	}
}
