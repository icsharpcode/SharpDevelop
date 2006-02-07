// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Util;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests that exit codes are read correctly by the
	/// process runner.
	/// </summary>
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class ExitCodeTestFixture : ConsoleAppTestFixtureBase
	{
		[Test]
		public void NonZeroExitCode()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
			
			int expectedExitCode = 1;
						
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");

			runner.Start(GetConsoleAppFileName(), String.Concat("-exitcode:", expectedExitCode.ToString()));
			runner.WaitForExit();
			
			Assert.AreEqual(expectedExitCode, runner.ExitCode, "Exit code is incorrect.");
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
		}
	}
}
