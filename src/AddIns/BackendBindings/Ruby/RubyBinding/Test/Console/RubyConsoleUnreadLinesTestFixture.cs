// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

using ICSharpCode.RubyBinding;
using IronRuby.Hosting;
using IronRuby.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the RubyConsole's GetUnreadLines method.
	/// </summary>
	[TestFixture]
	public class RubyConsoleUnreadLinesTestFixture
	{
		RubyConsole rubyConsole;
		MockConsoleTextEditor textEditor;
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockConsoleTextEditor();
			rubyConsole = new RubyConsole(textEditor, null);
		}
		
		[Test]
		public void NoUnreadLinesAtStart()
		{
			Assert.AreEqual(0, rubyConsole.GetUnreadLines().Length);
		}
	
		[Test]
		public void HasUnreadLines()
		{
			Assert.IsFalse(rubyConsole.IsLineAvailable);
		}
		
		[Test]
		public void AddOneLine()
		{
			textEditor.RaisePreviewKeyDownEvent(System.Windows.Input.Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(System.Windows.Input.Key.Enter);
			
			string[] expectedLines = new string[] {"A"};
			
			Assert.AreEqual(expectedLines, rubyConsole.GetUnreadLines());
			Assert.IsTrue(rubyConsole.IsLineAvailable);
		}
	}
}
