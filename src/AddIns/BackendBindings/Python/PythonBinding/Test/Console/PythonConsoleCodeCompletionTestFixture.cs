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
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// When the dot character is typed in after an object the code completion window should appear.
	/// </summary>
	[TestFixture]
	public class PythonConsoleCodeCompletionTestFixture
	{
		MockTextEditor textEditor;
		PythonConsole console;
		string prompt = ">>> ";
		bool showCompletionWindowCalledBeforeDotTypedIn;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			textEditor = new MockTextEditor();
			console = new PythonConsole(textEditor, null);
			console.WriteLine(prompt, Style.Prompt);
									
			textEditor.RaiseKeyPressEvents("__builtins__");
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
		public void PythonConsoleCompletionDataProviderPassedToShowCompletionWindowMethod()
		{
			Assert.IsInstanceOf(typeof(PythonConsoleCompletionDataProvider), textEditor.CompletionDataProvider);
		}
	}
}
