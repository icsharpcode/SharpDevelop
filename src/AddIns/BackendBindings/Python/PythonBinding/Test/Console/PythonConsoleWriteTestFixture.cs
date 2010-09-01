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
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests that the PythonConsole Write method correctly update the text editor.
	/// </summary>
	[TestFixture]
	public class PythonConsoleWriteTestFixture : PythonConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			base.CreatePythonConsole();
			MockConsoleTextEditor.Text = String.Empty;			
		}
		
		[Test]
		public void WriteLine()
		{
			TestablePythonConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void WriteLineWithText()
		{
			TestablePythonConsole.WriteLine("test", Style.Out);
			Assert.AreEqual("test" + Environment.NewLine, MockConsoleTextEditor.Text);
		}	
		
		[Test]
		public void TwoWrites()
		{
			TestablePythonConsole.Write("a", Style.Out);
			TestablePythonConsole.Write("b", Style.Out);
			Assert.AreEqual("ab", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void DoesNotHasLinesWaitingToBeRead()
		{
			Assert.IsFalse(TestablePythonConsole.IsLineAvailable);
		}
	}
}
