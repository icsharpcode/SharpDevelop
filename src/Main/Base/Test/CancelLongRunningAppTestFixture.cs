// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class CancelLongRunningAppTestFixture : ConsoleAppTestFixtureBase
	{
		ProcessRunner runner;
		
		[SetUp]
		public void Init()
		{
			runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
		}
		
		[Test]
		public void Cancel()
		{
			runner.Start(GetConsoleAppFileName(), "-forever");
			
			string processName = Path.GetFileName(GetConsoleAppFileName());
			processName = Path.ChangeExtension(processName, null);
			
			// Check console app is running.
			Process[] runningProcesses = Process.GetProcessesByName(processName);
			Assert.AreEqual(1, runningProcesses.Length, "Process is not running.");
			
			Assert.IsTrue(runner.IsRunning, "IsRunning should be true.");
			runner.Kill();
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
			
			// Check console app has been shutdown.
			runningProcesses = Process.GetProcessesByName(processName);
			if (runningProcesses.Length == 1) {
				// Windows kills the process asynchronously, so sometimes we have to wait a bit
				if (!runningProcesses[0].WaitForExit(2500))
					Assert.Fail("Process should have stopped.");
			} else {
				Assert.AreEqual(0, runningProcesses.Length, "Process should have stopped.");
			}
		}
	}
}
