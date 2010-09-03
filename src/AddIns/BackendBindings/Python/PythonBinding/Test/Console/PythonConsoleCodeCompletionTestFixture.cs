// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// When the dot character is typed in after an object the code completion window should appear.
	/// </summary>
	[TestFixture]
	public class PythonConsoleCodeCompletionTestFixture : PythonConsoleTestsBase
	{
		string prompt = ">>> ";
		bool showCompletionWindowCalledBeforeDotTypedIn;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.CreatePythonConsole();
			TestablePythonConsole.WriteLine(prompt, Style.Prompt);
			
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
 			showCompletionWindowCalledBeforeDotTypedIn = MockConsoleTextEditor.IsShowCompletionWindowCalled;
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.OemPeriod);		
		}
		
		[Test]
		public void ShowCompletionWindowCalled()
		{
			Assert.IsTrue(MockConsoleTextEditor.IsShowCompletionWindowCalled);
		}

		[Test]
		public void ShowCompletionWindowNotCalledBeforeDotTypedIn()
		{
			Assert.IsFalse(showCompletionWindowCalledBeforeDotTypedIn);
		}
		
		[Test]
		public void ScriptingConsoleCompletionDataProviderPassedToShowCompletionWindowMethod()
		{
			Assert.IsInstanceOf(typeof(ScriptingConsoleCompletionDataProvider), MockConsoleTextEditor.CompletionProviderPassedToShowCompletionWindow);
		}
	}
}
