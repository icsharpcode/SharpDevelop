// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading;

using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class ProcessExitedTestFixture : ConsoleAppTestFixtureBase
	{
		/// <summary>
		/// Tests the Runner.ProcessExit event works.
		/// </summary>
		[Test]
		public void ProcessExitEvent()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = GetConsoleAppFileName().GetParentDirectory();
			
			string echoText = "Test";
			
			runner.Start(GetConsoleAppFileName(), String.Concat("-echo:", echoText));
			bool exited = runner.WaitForExitAsync().Wait(2500);
			
			Assert.IsTrue(exited, "Timed out waiting for exit event.");
			Assert.AreEqual(0, runner.ExitCode, "Exit code should be zero.");
		}
	}
}
