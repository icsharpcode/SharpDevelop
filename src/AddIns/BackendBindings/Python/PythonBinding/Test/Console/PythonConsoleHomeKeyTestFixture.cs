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
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// The Home Key should return the user to the start of the line after the prompt.
	/// </summary>
	[TestFixture]
	public class PythonConsoleHomeKeyTestFixture
	{
		MockConsoleTextEditor textEditor;
		PythonConsole console;
		string prompt = ">>> ";

		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			console = new PythonConsole(textEditor, null);
			console.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void HomeKeyPressedWhenNoUserTextInConsole()
		{
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Home);
		
			int expectedColumn = prompt.Length;
			Assert.AreEqual(expectedColumn, textEditor.Column);
		}
		
		[Test]
		public void HomeKeyPressedWhenTextInConsole()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Home);

			int expectedColumn = prompt.Length;
			Assert.AreEqual(expectedColumn, textEditor.Column);
		}		
	}
}
