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
	public class RubyConsoleWriteTestFixture : RubyConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			base.CreateRubyConsole();
		}
		
		[Test]
		public void WriteLine()
		{
			TestableRubyConsole.WriteLine();
			Assert.AreEqual(Environment.NewLine, MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void WriteLineWithText()
		{
			TestableRubyConsole.WriteLine("test", Style.Out);
			Assert.AreEqual("test" + Environment.NewLine, MockConsoleTextEditor.Text);
		}	
		
		[Test]
		public void TwoWrites()
		{
			TestableRubyConsole.Write("a", Style.Out);
			TestableRubyConsole.Write("b", Style.Out);
			Assert.AreEqual("ab", MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void DoesNotHasLinesWaitingToBeRead()
		{
			Assert.IsFalse(TestableRubyConsole.IsLineAvailable);
		}
	}
}
