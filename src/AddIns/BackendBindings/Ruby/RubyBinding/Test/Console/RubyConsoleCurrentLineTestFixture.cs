// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Tests the RubyConsole's GetCurrentLine method.
	/// </summary>
	[TestFixture]
	public class RubyConsoleCurrentLineTestFixture
	{
		RubyConsole RubyConsole;
		MockTextEditor textEditor;
		string prompt = ">>> ";
		
		[SetUp]
		public void Init()
		{
			textEditor = new MockTextEditor();
			RubyConsole = new RubyConsole(textEditor, null);
			RubyConsole.Write(prompt, Style.Prompt);
		}
		
		[Test]
		public void CurrentLineIsEmpty()
		{
			Assert.AreEqual(String.Empty, RubyConsole.GetCurrentLine());
		}
		
		[Test]
		public void SingleCharacterAddedToTextEditor()
		{
			textEditor.RaiseKeyPressEvent('a');
			Assert.AreEqual("a", RubyConsole.GetCurrentLine());
		}
	}
}
