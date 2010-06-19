// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using ICSharpCode.RubyBinding;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests that the RubyConsole Write method correctly update the text editor.
	/// </summary>
	[TestFixture]
	public class RubyConsoleWriteTestFixture
	{
		RubyConsole rubyConsole;
		MockConsoleTextEditor mockTextEditor;
		
		[SetUp]
		public void Init()
		{
			mockTextEditor = new MockConsoleTextEditor();
			mockTextEditor.Text = String.Empty;			
			rubyConsole = new RubyConsole(mockTextEditor, null);
		}
		
		[Test]
		public void WriteLine()
		{
			rubyConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, mockTextEditor.Text);
		}
		
		[Test]
		public void WriteLineWithText()
		{
			rubyConsole.WriteLine("test", Style.Out);
			Assert.AreEqual("test" + Environment.NewLine, mockTextEditor.Text);
		}	
		
		[Test]
		public void TwoWrites()
		{
			rubyConsole.Write("a", Style.Out);
			rubyConsole.Write("b", Style.Out);
			Assert.AreEqual("ab", mockTextEditor.Text);
		}
		
		[Test]
		public void DoesNotHasLinesWaitingToBeRead()
		{
			Assert.IsFalse(rubyConsole.IsLineAvailable);
		}
	}
}
