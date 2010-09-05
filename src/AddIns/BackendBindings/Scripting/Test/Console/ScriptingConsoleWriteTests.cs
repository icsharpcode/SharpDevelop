// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests that the ScriptingConsole Write method correctly update the text editor.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleWriteTestFixture : ScriptingConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			base.CreateConsole();
			MockConsoleTextEditor.Text = String.Empty;			
		}
		
		[Test]
		public void WriteLine()
		{
			TestableScriptingConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void WriteLineWithText()
		{
			TestableScriptingConsole.WriteLine("test", ScriptingStyle.Out);
			string expectedText = "test" + Environment.NewLine;
			Assert.AreEqual(expectedText, MockConsoleTextEditor.Text);
		}	
		
		[Test]
		public void TwoWrites()
		{
			TestableScriptingConsole.Write("a", ScriptingStyle.Out);
			TestableScriptingConsole.Write("b", ScriptingStyle.Out);
			Assert.AreEqual("ab", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void DoesNotHasLinesWaitingToBeRead()
		{
			Assert.IsFalse(TestableScriptingConsole.IsLineAvailable);
		}
	}
}
