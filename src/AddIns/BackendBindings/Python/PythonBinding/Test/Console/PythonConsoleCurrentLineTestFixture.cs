// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
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
		TestablePythonConsole pythonConsole;
		MockConsoleTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			pythonConsole = new TestablePythonConsole();
			pythonConsole.Write(prompt, Style.Prompt);
			textEditor = pythonConsole.MockConsoleTextEditor;
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
