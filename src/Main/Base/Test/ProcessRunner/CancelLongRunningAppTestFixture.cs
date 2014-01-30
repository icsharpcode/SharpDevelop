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
using System.Diagnostics;
using System.IO;
using System.Threading;

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
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
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
			
			//Assert.IsTrue(runner.IsRunning, "IsRunning should be true.");
			runner.Kill();
			//Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
			
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
