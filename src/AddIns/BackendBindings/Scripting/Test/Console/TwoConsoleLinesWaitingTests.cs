// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
