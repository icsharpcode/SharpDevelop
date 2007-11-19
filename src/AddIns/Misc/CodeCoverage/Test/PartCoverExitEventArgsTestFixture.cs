// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.CodeCoverage;
using NUnit.Framework;

namespace ICSharpCode.CodeCoverage.Tests
{
	/// <summary>
	/// Tests the PartCoverExitEventArgs class.
	/// </summary>
	[TestFixture]
	public class PartCoverExitEventArgsTestFixture
	{
		PartCoverExitEventArgs eventArgs;
		string output = "Test";
		string error = "Error";
		int exitCode = -1;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			eventArgs = new PartCoverExitEventArgs(output, error, exitCode);
		}
		
		[Test]
		public void OutputText()
		{
			Assert.AreEqual(output, eventArgs.Output);
		}

		[Test]
		public void ErrorText()
		{
			Assert.AreEqual(error, eventArgs.Error);
		}

		[Test]
		public void ExitCode()
		{
			Assert.AreEqual(exitCode, eventArgs.ExitCode);
		}
	}
}
