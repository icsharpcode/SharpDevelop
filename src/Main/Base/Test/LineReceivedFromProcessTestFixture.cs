// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using NUnit.Framework;
using ICSharpCode.SharpDevelop.Util;
using System;
using System.Collections;
using System.IO;

namespace ICSharpCode.SharpDevelop.Tests
{
	/// <summary>
	/// Tests the <see cref="ProcessRunner.LineReceived"/> event.
	/// </summary>
	[TestFixture]
	public class LineReceivedFromProcessTestFixture : ConsoleAppTestFixtureBase
	{
		ProcessRunner runner;
		ArrayList lines;
		
		[SetUp]
		public void Init()
		{
			lines = new ArrayList();
			runner = new ProcessRunner();
			runner.WorkingDirectory = Path.GetDirectoryName(GetConsoleAppFileName());
		}
		
		[Test]
		public void SingleLineOutput()
		{			
			string echoText = "Test";
			string expectedOutput = String.Concat(echoText, "\r\n");
			
			runner.OutputLineReceived += new LineReceivedEventHandler(OutputLineReceived);
			
			runner.Start(GetConsoleAppFileName(), String.Concat("-echo:", echoText));
			runner.WaitForExit();
			
			Assert.AreEqual(0, runner.ExitCode, "Exit code should be zero.");
			Assert.AreEqual(expectedOutput, runner.StandardOutput, "Should have some output.");
			Assert.AreEqual(String.Empty, runner.StandardError, "Should not be any error output.");			
			Assert.AreEqual(1, lines.Count, "Should only have one output line.");
			Assert.AreEqual(echoText, lines[0], "Line received is incorrect.");
		}
		
		void OutputLineReceived(object sender, LineReceivedEventArgs e)
		{
			lines.Add(e.Line);
		}
	}
}
