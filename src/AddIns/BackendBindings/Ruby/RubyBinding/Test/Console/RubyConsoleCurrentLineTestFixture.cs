// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the RubyConsole's GetCurrentLine method.
	/// </summary>
	[TestFixture]
	public class RubyConsoleCurrentLineTestFixture
	{
		TestableRubyConsole rubyConsole;
		MockConsoleTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			rubyConsole = new TestableRubyConsole();
			rubyConsole.Write(prompt, Style.Prompt);
			textEditor = rubyConsole.MockConsoleTextEditor;
		}
		
		[Test]
		public void CurrentLineIsEmpty()
		{
			Assert.AreEqual(String.Empty, rubyConsole.GetCurrentLine());
		}
		
		[Test]
		public void SingleCharacterAddedToTextEditor()
		{
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			Assert.AreEqual("A", rubyConsole.GetCurrentLine());
		}
	}
}
