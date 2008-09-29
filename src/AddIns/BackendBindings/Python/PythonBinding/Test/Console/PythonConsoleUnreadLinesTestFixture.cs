// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using IronPython.Hosting;
using IronPython.Runtime;
using NUnit.Framework;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the PythonConsole's GetUnreadLines method.
	/// </summary>
	[TestFixture]
	public class PythonConsoleUnreadLinesTestFixture
	{
		PythonConsole pythonConsole;
		MockTextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
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
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			
			string[] expectedLines = new string[] {"a"};
			
			Assert.AreEqual(expectedLines, pythonConsole.GetUnreadLines());
			Assert.IsTrue(pythonConsole.IsLineAvailable);
		}
	}
}
