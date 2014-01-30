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
		public void Write_WritePromptWhenScrollToEndWhenTextWritten_TextEditorIsScrolled()
		{
			TestableScriptingConsole.ScrollToEndWhenTextWritten = true;
			TestableScriptingConsole.Write("a", ScriptingStyle.Prompt);
			
			Assert.IsTrue(FakeConsoleTextEditor.IsScrollToEndCalled);
		}
		
		[Test]
		public void Write_WritePromptWhenScrollToEndWhenTextWrittenIsFalse_TextEditorIsNotScrolled()
		{
			TestableScriptingConsole.ScrollToEndWhenTextWritten = false;
			TestableScriptingConsole.Write("a", ScriptingStyle.Prompt);
			
			Assert.IsFalse(FakeConsoleTextEditor.IsScrollToEndCalled);
		}
		
		[Test]
		public void Write_WriteErrorWhenScrollToEndWhenTextWrittenIsTrue_TextEditorIsScrolled()
		{
			TestableScriptingConsole.ScrollToEndWhenTextWritten = true;
			TestableScriptingConsole.Write("a", ScriptingStyle.Error);
			
			Assert.IsTrue(FakeConsoleTextEditor.IsScrollToEndCalled);
		}
		
		[Test]
		public void ScrollToEndWhenTextWritten_NewInstance_IsTrue()
		{
			Assert.IsTrue(TestableScriptingConsole.ScrollToEndWhenTextWritten);
		}
		
		[Test]
		public void Clear_TextEditorHasText_RemovesAllText()
		{
			TestableScriptingConsole.Write("a", ScriptingStyle.Prompt);
			TestableScriptingConsole.Clear();
			
			bool cleared = FakeConsoleTextEditor.IsClearCalled;
			
			Assert.IsTrue(cleared);
		}
	}
}
