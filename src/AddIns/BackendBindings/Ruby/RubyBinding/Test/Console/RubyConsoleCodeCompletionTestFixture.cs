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
	public class RubyConsoleCodeCompletionTestFixture : RubyConsoleTestsBase
	{
		string prompt = ">>> ";
		bool showCompletionWindowCalledBeforeDotTypedIn;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			base.CreateRubyConsole();
			TestableRubyConsole.WriteLine(prompt, Style.Prompt);
			
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
		public void RubyConsoleCompletionDataProviderPassedToShowCompletionWindowMethod()
		{
			Assert.IsInstanceOf(typeof(RubyConsoleCompletionDataProvider), MockConsoleTextEditor.CompletionProviderPassedToShowCompletionWindow);
		}
	}
}
