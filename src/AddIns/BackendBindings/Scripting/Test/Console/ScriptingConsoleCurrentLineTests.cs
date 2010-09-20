// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the ScriptingConsole's GetCurrentLine method.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleCurrentLineTests
	{
		TestableScriptingConsole console;
		FakeConsoleTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			console = new TestableScriptingConsole();
			console.Write(prompt, ScriptingStyle.Prompt);
			textEditor = console.FakeConsoleTextEditor;
		}
		
		[Test]
		public void CurrentLineIsEmpty()
		{
			Assert.AreEqual(String.Empty, console.GetCurrentLine());
		}
		
		[Test]
		public void SingleCharacterAddedToTextEditor()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			Assert.AreEqual("A", console.GetCurrentLine());
		}
	}
}
