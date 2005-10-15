// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using ICSharpCode.NAntAddIn;

namespace ICSharpCode.NAntAddIn.Tests
{
	/// <summary>
	/// Checks that the <see cref="ProcessRunner"/> responds
	/// correctly if it has not been started.
	/// </summary>
	[TestFixture]
	//[Ignore("Ignoring since need to run ConsoleApp.exe")]
	public class ProcessRunnerNotStartedTestFixture
	{
		ProcessRunner runner;
		
		[SetUp]
		public void Init()
		{
			runner = new ProcessRunner();
		}
		
		[Test]
		public void ExitCode()
		{
			Assert.AreEqual(0, runner.ExitCode, "Exit code should be zero.");
		}
		
		[Test]
		public void StandardOutput()
		{
			Assert.AreEqual(String.Empty, runner.StandardOutput, "Standard output should be empty.");			
		}
		
		[Test]
		public void StandardError()
		{
			Assert.AreEqual(String.Empty, runner.StandardError, "Standard error should be empty.");			
		}		
		
		[Test]
		[ExpectedException(typeof(ProcessRunnerException), "No process running.")]
		public void WaitForExit()
		{
			runner.WaitForExit();
		}
	}
}
