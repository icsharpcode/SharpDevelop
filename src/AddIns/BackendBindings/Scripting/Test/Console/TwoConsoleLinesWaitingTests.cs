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
using System.Threading;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;
using Input = System.Windows.Input;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Ensures that both lines of text can be read from the console if they are written
	/// before ReadLine is called.
	/// </summary>
	[TestFixture]
	public class TwoConsoleLinesWaitingTests : ScriptingConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			CreateConsole();
		}
		
		[Test]
		public void IsLineAvailable_ThreeCharactersWritten_ReturnsFalse()
		{
			WriteThreeCharactersToTextEditor();
			bool result = TestableScriptingConsole.IsLineAvailable;
			Assert.IsFalse(result);
		}
		
		void WriteThreeCharactersToTextEditor()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Input.Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Input.Key.B);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Input.Key.C);
		}
		
		[Test]
		public void ReadLine_WriteThreeCharactersFollowedByEnterKey_ReturnsFirstThreeCharacters()
		{
			WriteThreeCharactersFollowedByEnterKey();
			string line = TestableScriptingConsole.ReadLine(0);
			
			string expectedLine = "ABC";
			Assert.AreEqual(expectedLine, line);
		}
		
		void WriteThreeCharactersFollowedByEnterKey()
		{
			WriteThreeCharactersToTextEditor();
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Input.Key.Enter);
		}
		
		[Test]
		public void ReadLine_WriteTwoLinesWithThreeCharactersInEachLine_SecondLineReadReturnsSecondLinesCharacters()
		{
			WriteTwoLinesEachFollowedByEnterKey();

			TestableScriptingConsole.ReadLine(0);
			string secondLine = TestableScriptingConsole.ReadLine(0);
			
			string expectedSecondLine = "DEF";
			Assert.AreEqual(expectedSecondLine, secondLine);
		}
		
		void WriteTwoLinesEachFollowedByEnterKey()
		{
			WriteThreeCharactersFollowedByEnterKey();
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Input.Key.D);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Input.Key.E);
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Input.Key.F);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Input.Key.Enter);
		}

		[Test]
		public void IsLineAvailable_ThreeCharactersAndEnterKeyWrittenToTextEditor_ReturnsTrue()
		{
			WriteThreeCharactersFollowedByEnterKey();
			bool result = TestableScriptingConsole.IsLineAvailable;
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void IsLineAvailable_TwoLinesRead_ReturnsFalse()
		{
			WriteTwoLinesEachFollowedByEnterKey();
			TestableScriptingConsole.ReadLine(0);
			TestableScriptingConsole.ReadLine(0);
			bool result = TestableScriptingConsole.IsLineAvailable;
			
			Assert.IsFalse(result);
		}
	}
}
