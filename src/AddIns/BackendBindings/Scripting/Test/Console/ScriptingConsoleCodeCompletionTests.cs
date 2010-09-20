// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// When the dot character is typed in after an object the code completion window should appear.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleCodeCompletionTests : ScriptingConsoleTestsBase
	{
		string prompt = ">>> ";
		bool showCompletionWindowCalledBeforeDotTypedIn;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.CreateConsole();
			TestableScriptingConsole.WriteLine(prompt, ScriptingStyle.Prompt);
			
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
 			showCompletionWindowCalledBeforeDotTypedIn = FakeConsoleTextEditor.IsShowCompletionWindowCalled;
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.OemPeriod);		
		}
		
		[Test]
		public void ShowCompletionWindowCalled()
		{
			Assert.IsTrue(FakeConsoleTextEditor.IsShowCompletionWindowCalled);
		}

		[Test]
		public void ShowCompletionWindowNotCalledBeforeDotTypedIn()
		{
			Assert.IsFalse(showCompletionWindowCalledBeforeDotTypedIn);
		}
		
		[Test]
		public void ScriptingConsoleCompletionDataProviderPassedToShowCompletionWindowMethod()
		{
			Assert.IsInstanceOf(typeof(ScriptingConsoleCompletionDataProvider), FakeConsoleTextEditor.CompletionProviderPassedToShowCompletionWindow);
		}
	}
}
