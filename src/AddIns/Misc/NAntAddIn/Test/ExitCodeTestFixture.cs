// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.NAntAddIn;
using NUnit.Framework;
using System;
using System.IO;

namespace ICSharpCode.NAntAddIn.Tests
{
	/// <summary>
	/// Tests that exit codes are read correctly by the
	/// process runner.
	/// </summary>
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class ExitCodeTestFixture
	{
		[Test]
		public void NonZeroExitCode()
		{
			ProcessRunner runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(Config.ConsoleAppFilename);
			
			int expectedExitCode = 1;
						
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");

			runner.Start(Config.ConsoleAppFilename, String.Concat("-exitcode:", expectedExitCode.ToString()));
			runner.WaitForExit();
			
			Assert.AreEqual(expectedExitCode, runner.ExitCode, "Exit code is incorrect.");
			Assert.IsFalse(runner.IsRunning, "IsRunning should be false.");
		}
	}
}
