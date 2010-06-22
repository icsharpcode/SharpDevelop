// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

using ICSharpCode.PythonBinding;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole's GetUnreadLines method.
	/// </summary>
	[TestFixture]
	public class PythonConsoleUnreadLinesTestFixture
	{
		PythonConsole pythonConsole;
		MockConsoleTextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			pythonConsole = new PythonConsole(textEditor, null);
		}
		
		[Test]
		public void NoUnreadLinesAtStart()
		{
			Assert.AreEqual(0, pythonConsole.GetUnreadLines().Length);
		}
	
		[Test]
		public void HasUnreadLines()
		{
			Assert.IsFalse(pythonConsole.IsLineAvailable);
		}
		
		[Test]
		public void AddOneLine()
		{
			textEditor.RaisePreviewKeyDownEvent(System.Windows.Input.Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(System.Windows.Input.Key.Enter);
			
			string[] expectedLines = new string[] {"A"};
			
			Assert.AreEqual(expectedLines, pythonConsole.GetUnreadLines());
			Assert.IsTrue(pythonConsole.IsLineAvailable);
		}
	}
}
