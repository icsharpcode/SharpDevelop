// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

using ICSharpCode.RubyBinding;
using ICSharpCode.TextEditor;
using IronRuby.Hosting;
using IronRuby.Runtime;
using NUnit.Framework;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Basic tests for the RubyConsoleHost class.
	/// </summary>
	[TestFixture]
	public class RubyConsoleHostTests
	{
		DerivedRubyConsoleHost host;
		TextEditorControl textEditorControl;
		TextEditor textEditor;
		
		[TestFixtureSetUp]
		public void Init()
		{
			textEditorControl = new TextEditorControl();
			textEditor = new TextEditor(textEditorControl);
			host = new DerivedRubyConsoleHost(textEditor);
			
			//ScriptRuntime runtime = IronRuby.Hosting.Ruby.CreateRuntime();
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			host.Dispose();
			textEditorControl.Dispose();
		}
		
		[Test]
		public void RubyConsoleHostIsMicrosoftScriptingConsoleHostType()
		{
			Assert.IsInstanceOf(typeof(ConsoleHost), host);
		}
		
		[Test]
		public void OptionsParserCreatedIsRubyOptionsParser()
		{
			OptionsParser parser = host.CallCreateOptionsParser();
			Assert.IsInstanceOf(typeof(RubyOptionsParser), parser);
		}
		
		[Test]
		public void CommandLineCreatedIsRubyCommandLine()
		{
			CommandLine commandLine = host.CallCreateCommandLine();
			Assert.IsInstanceOf(typeof(RubyCommandLine), commandLine);
		}

		[Test]
		public void ConsoleCreatedIsRubyConsole()
		{
			IConsole console = host.CallCreateConsole(null, null, null);
			Assert.IsInstanceOf(typeof(RubyConsole), console);
		}
		
		[Test]
		public void RubyLanguageIsSetup()
		{
			LanguageSetup setup = host.CallCreateLanguageSetup();
			Assert.AreEqual("IronRuby", setup.DisplayName);
		}
		
		[Test]
		public void ConsoleHostImplementsIDisposable()
		{
			Assert.IsNotNull(host as IDisposable);
		}
		
		/// <summary>
		/// When the console is disposed calling ReadLine returns null.
		/// </summary>
		[Test]
		public void HostDisposesRubyConsole()
		{
			DerivedRubyConsoleHost host = new DerivedRubyConsoleHost(new MockTextEditor());
			RubyConsole console = host.CallCreateConsole(null, null, null) as RubyConsole;
			host.Dispose();

			Assert.IsNull(console.ReadLine(0));
		}
		
		/// <summary>
		/// Makes sure the Dispose method checks if the Ruby console is null before trying to dispose it.
		/// </summary>
		[Test]
		public void DisposingRubyConsoleHostWithoutCreatingRubyConsole()
		{
			RubyConsoleHost host = new RubyConsoleHost(new MockTextEditor());
			host.Dispose();
		}
		
		[Test]
		public void DefaultOutputStreamReplacedByCustomStreamClass()
		{
			Assert.IsNotNull(host.OutputStream);
		}
		
		[Test]
		public void ProviderIsRubyContext()
		{
			Assert.AreEqual(typeof(RubyContext), host.GetProvider());
		}		
	}
}