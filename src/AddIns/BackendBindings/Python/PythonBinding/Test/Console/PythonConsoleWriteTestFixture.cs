// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.PythonBinding;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests that the PythonConsole Write method correctly update the text editor.
	/// </summary>
	[TestFixture]
	public class PythonConsoleWriteTestFixture
	{
		PythonConsole pythonConsole;
		MockTextEditor mockTextEditor;
		
		[SetUp]
		public void Init()
		{
			mockTextEditor = new MockTextEditor();
			mockTextEditor.Text = String.Empty;			
			pythonConsole = new PythonConsole(mockTextEditor, null);
		}
		
		[Test]
		public void WriteLine()
		{
			pythonConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, mockTextEditor.Text);
		}
		
		[Test]
		public void WriteLineWithText()
		{
			pythonConsole.WriteLine("test", Style.Out);
			Assert.AreEqual("test" + Environment.NewLine, mockTextEditor.Text);
		}	
		
		[Test]
		public void TwoWrites()
		{
			pythonConsole.Write("a", Style.Out);
			pythonConsole.Write("b", Style.Out);
			Assert.AreEqual("ab", mockTextEditor.Text);
		}
		
		[Test]
		public void DoesNotHasLinesWaitingToBeRead()
		{
			Assert.IsFalse(pythonConsole.IsLineAvailable);
		}
	}
}
