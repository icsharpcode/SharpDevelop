// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the RubyConsole's command line history.
	/// </summary>
	[TestFixture]
	public class RubyConsoleCommandLineHistoryTestFixture : RubyConsoleTestsBase
	{
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.Write(prompt, Style.Prompt);
			
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableRubyConsole.Write(prompt, Style.Prompt);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.B);
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.C);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			TestableRubyConsole.Write(prompt, Style.Prompt);
		}

		[Test]
		public void UpArrowKeyPressed()
		{
			Assert.IsTrue(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up));
		}
		
		[Test]
		public void CurrentLineAfterUpArrowKeyPressed()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual("BC", TestableRubyConsole.GetCurrentLine());
		}
		
		[Test]
		public void TextEditorCursorIsAtEndOfLineAfterUpArrowKeyPressed()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 2, MockConsoleTextEditor.Column);
		}
		
		[Test]
		public void TextAfterUpArrowKeyPressedTwiceThenDownArrowKey()
		{
			UpArrowKeyPressedTwiceThenDownArrowKey();
			Assert.AreEqual("BC", TestableRubyConsole.GetCurrentLine());
		}

		[Test]
		public void TextEditorCursorAfterUpArrowKeyPressedTwice()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			Assert.AreEqual(prompt.Length + 1, MockConsoleTextEditor.Column);
		}
		
		[Test]
		public void DownArrowKeyHandled()
		{
			Assert.IsTrue(MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down));
		}
		
		void UpArrowKeyPressedTwiceThenDownArrowKey()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Up);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Down);
		}
	}
}
