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
			FakeConsoleTextEditor.Text = String.Empty;			
		}
		
		[Test]
		public void WriteLine_WriteLine_NewLineWritten()
		{
			TestableScriptingConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, FakeConsoleTextEditor.Text);
		}
		
		[Test]
		public void WriteLine_WriteLineWithText_TextPlusNewLineWritten()
		{
			TestableScriptingConsole.WriteLine("test", ScriptingStyle.Out);
			string expectedText = "test" + Environment.NewLine;
			Assert.AreEqual(expectedText, FakeConsoleTextEditor.Text);
		}	
		
		[Test]
		public void Write_TwoWrites_BothWrittenToTextEditor()
		{
			TestableScriptingConsole.Write("a", ScriptingStyle.Out);
			TestableScriptingConsole.Write("b", ScriptingStyle.Out);
			Assert.AreEqual("ab", FakeConsoleTextEditor.Text);
		}
		
		[Test]
		public void Constructor_NewInstance_DoesNotHaveLinesWaitingToBeRead()
		{
			Assert.IsFalse(TestableScriptingConsole.IsLineAvailable);
		}
		
		[Test]
		public void Write_WritePromptWhenScrollToPromptIsTrue_TextEditorIsScrolled()
		{
			TestableScriptingConsole.ScrollToEndWhenPromptWritten = true;
			TestableScriptingConsole.Write("a", ScriptingStyle.Prompt);
			
			Assert.IsTrue(FakeConsoleTextEditor.IsScrollToEndCalled);
		}
		
		[Test]
		public void Write_WritePromptWhenScrollToPromptIsFalse_TextEditorIsNotScrolled()
		{
			TestableScriptingConsole.ScrollToEndWhenPromptWritten = false;
			TestableScriptingConsole.Write("a", ScriptingStyle.Prompt);
			
			Assert.IsFalse(FakeConsoleTextEditor.IsScrollToEndCalled);
		}
		
		[Test]
		public void Write_WriteErrorWhenScrollToPromptIsTrue_TextEditorIsNotScrolled()
		{
			TestableScriptingConsole.ScrollToEndWhenPromptWritten = true;
			TestableScriptingConsole.Write("a", ScriptingStyle.Error);
			
			Assert.IsFalse(FakeConsoleTextEditor.IsScrollToEndCalled);
		}
		
		[Test]
		public void ScrollToEndWhenPromptWritten_NewInstance_IsTrue()
		{
			Assert.IsTrue(TestableScriptingConsole.ScrollToEndWhenPromptWritten);
		}
	}
}
