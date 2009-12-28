// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// When the dot character is typed in after an object the code completion window should appear.
	/// </summary>
	[TestFixture]
	public class RubyConsoleCodeCompletionTestFixture
	{
		MockTextEditor textEditor;
		RubyConsole console;
		string prompt = ">>> ";
		bool showCompletionWindowCalledBeforeDotTypedIn;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditor = new MockTextEditor();
			console = new RubyConsole(textEditor, null);
			console.WriteLine(prompt, Style.Prompt);
									
			textEditor.RaiseKeyPressEvents("a");
			showCompletionWindowCalledBeforeDotTypedIn = textEditor.IsShowCompletionWindowCalled;
			textEditor.RaiseKeyPressEvent('.');		
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
