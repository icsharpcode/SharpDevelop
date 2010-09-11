// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading;
using ICSharpCode.Scripting;
using NUnit.Framework;
using ICSharpCode.Scripting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// The Home Key should return the user to the start of the line after the prompt.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleHomeKeyTests : ScriptingConsoleTestsBase
	{
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			base.CreateConsole();
			TestableScriptingConsole.Write(prompt, ScriptingStyle.Prompt);
		}
		
		[Test]
		public void HomeKeyPressedWhenNoUserTextInConsole()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Home);
		
			int expectedColumn = prompt.Length;
			Assert.AreEqual(expectedColumn, FakeConsoleTextEditor.Column);
		}
		
		[Test]
		public void HomeKeyPressedWhenTextInConsole()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Home);

			int expectedColumn = prompt.Length;
			Assert.AreEqual(expectedColumn, FakeConsoleTextEditor.Column);
		}		
	}
}
