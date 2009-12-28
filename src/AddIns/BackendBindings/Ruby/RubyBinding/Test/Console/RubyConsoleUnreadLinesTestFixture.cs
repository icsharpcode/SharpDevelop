// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using IronRuby.Hosting;
using IronRuby.Runtime;
using NUnit.Framework;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the RubyConsole's GetUnreadLines method.
	/// </summary>
	[TestFixture]
	public class RubyConsoleUnreadLinesTestFixture
	{
		RubyConsole RubyConsole;
		MockTextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			RubyConsole = new RubyConsole(textEditor, null);
		}
		
		[Test]
		public void NoUnreadLinesAtStart()
		{
			Assert.AreEqual(0, RubyConsole.GetUnreadLines().Length);
		}
	
		[Test]
		public void HasUnreadLines()
		{
			Assert.IsFalse(RubyConsole.IsLineAvailable);
		}
		
		[Test]
		public void AddOneLine()
		{
			textEditor.RaiseKeyPressEvent('a');
			textEditor.RaiseDialogKeyPressEvent(Keys.Enter);
			
			string[] expectedLines = new string[] {"a"};
			
			Assert.AreEqual(expectedLines, RubyConsole.GetUnreadLines());
			Assert.IsTrue(RubyConsole.IsLineAvailable);
		}
	}
}
