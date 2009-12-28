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

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests that the RubyConsole Write method correctly update the text editor.
	/// </summary>
	[TestFixture]
	public class RubyConsoleWriteTestFixture
	{
		RubyConsole RubyConsole;
		MockTextEditor mockTextEditor;
		
		[SetUp]
		public void Init()
		{
			mockTextEditor = new MockTextEditor();
			mockTextEditor.Text = String.Empty;			
			RubyConsole = new RubyConsole(mockTextEditor, null);
		}
		
		[Test]
		public void WriteLine()
		{
			RubyConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, mockTextEditor.Text);
		}
		
		[Test]
		public void WriteLineWithText()
		{
			RubyConsole.WriteLine("test", Style.Out);
			Assert.AreEqual("test" + Environment.NewLine, mockTextEditor.Text);
		}	
		
		[Test]
		public void TwoWrites()
		{
			RubyConsole.Write("a", Style.Out);
			RubyConsole.Write("b", Style.Out);
			Assert.AreEqual("ab", mockTextEditor.Text);
		}
		
		[Test]
		public void DoesNotHasLinesWaitingToBeRead()
		{
			Assert.IsFalse(RubyConsole.IsLineAvailable);
		}
	}
}
