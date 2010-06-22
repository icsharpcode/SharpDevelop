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
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole's GetCurrentLine method.
	/// </summary>
	[TestFixture]
	public class PythonConsoleCurrentLineTestFixture
	{
		PythonConsole pythonConsole;
		MockConsoleTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			pythonConsole = new PythonConsole(textEditor, null);
			pythonConsole.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void CurrentLineIsEmpty()
		{
			Assert.AreEqual(String.Empty, pythonConsole.GetCurrentLine());
		}
		
		[Test]
		public void SingleCharacterAddedToTextEditor()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			Assert.AreEqual("A", pythonConsole.GetCurrentLine());
		}
	}
}
