// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.RubyBinding;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// When the dot character is typed in after an object the code completion window should appear.
	/// </summary>
	[TestFixture]
	public class RubyConsoleCodeCompletionTestFixture
	{
		MockConsoleTextEditor textEditor;
		RubyConsole console;
		string prompt = ">>> ";
		bool showCompletionWindowCalledBeforeDotTypedIn;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditor = new MockConsoleTextEditor();
			console = new RubyConsole(textEditor, null);
			console.WriteLine(prompt, Style.Prompt);
									
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			showCompletionWindowCalledBeforeDotTypedIn = textEditor.IsShowCompletionWindowCalled;
			textEditor.RaisePreviewKeyDownEvent(Key.OemPeriod);		
		}
		
		[Test]
		public void ShowCompletionWindowCalled()
		{
			Assert.IsTrue(textEditor.IsShowCompletionWindowCalled);
		}

		[Test]
		public void ShowCompletionWindowNotCalledBeforeDotTypedIn()
		{
			Assert.IsFalse(showCompletionWindowCalledBeforeDotTypedIn);
		}
		
		[Test]
		public void RubyConsoleCompletionDataProviderPassedToShowCompletionWindowMethod()
		{
			Assert.IsInstanceOf(typeof(RubyConsoleCompletionDataProvider), textEditor.CompletionDataProvider);
		}
	}
}
