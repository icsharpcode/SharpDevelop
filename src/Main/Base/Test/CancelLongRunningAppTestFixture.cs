// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

using ICSharpCode.SharpDevelop.Util;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class CancelLongRunningAppTestFixture : ConsoleAppTestFixtureBase
	{
		Mutex mutex;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			// Avoid test failure on build server when unit tests for several branches are run concurrently.
			bool createdNew;
			mutex = new Mutex(true, "CancelLongRunningAppTestFixture", out createdNew);
			if (!createdNew) {
				if (!mutex.WaitOne(10000)) {
					throw new Exception("Could not acquire mutex");
				}
			}
		}
		
		[TestFixtureTearDown]
		public void FixtureTearDown()
		{
			mutex.ReleaseMutex();
			mutex.Dispose();
		}
		
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
