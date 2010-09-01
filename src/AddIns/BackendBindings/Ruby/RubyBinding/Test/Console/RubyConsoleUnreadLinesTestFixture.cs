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
	public class RubyConsoleUnreadLinesTestFixture : RubyConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			base.CreateRubyConsole();
		}
		
		[Test]
		public void NoUnreadLinesAtStart()
		{
			Assert.AreEqual(0, TestableRubyConsole.GetUnreadLines().Length);
		}
	
		[Test]
		public void HasUnreadLines()
		{
			Assert.IsFalse(TestableRubyConsole.IsLineAvailable);
		}
		
		[Test]
		public void AddOneLine()
		{
			MockConsoleTextEditor.RaisePreviewKeyDownEvent(System.Windows.Input.Key.A);
			MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(System.Windows.Input.Key.Enter);
			
			string[] lines = TestableRubyConsole.GetUnreadLines();
			string[] expectedLines = new string[] {"A"};
			
			Assert.AreEqual(expectedLines, lines);
			Assert.IsTrue(TestableRubyConsole.IsLineAvailable);
		}
	}
}
