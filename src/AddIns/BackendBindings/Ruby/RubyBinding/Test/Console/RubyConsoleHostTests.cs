// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Console
{
	/// <summary>
	/// Basic tests for the RubyConsoleHost class.
	/// </summary>
	[TestFixture]
	public class RubyConsoleHostTests
	{
		DerivedRubyConsoleHost host;
		TextEditor textEditorControl;
		ScriptingConsoleTextEditor textEditor;
		
		[TestFixtureSetUp]
		public void Init()
		{
			textEditorControl = new TextEditor();
			textEditor = new ScriptingConsoleTextEditor(textEditorControl);
			host = new DerivedRubyConsoleHost(textEditor);
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			host.Dispose();
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
			Assert.IsInstanceOf(typeof(IronRuby.Hosting.RubyOptionsParser), parser);
		}
		
		[Test]
		public void CommandLineCreatedIsRubyCommandLine()
		{
			CommandLine commandLine = host.CallCreateCommandLine();
			Assert.IsInstanceOf(typeof(IronRuby.Hosting.RubyCommandLine), commandLine);
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
			DerivedRubyConsoleHost host = new DerivedRubyConsoleHost(new FakeConsoleTextEditor());
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
			RubyConsoleHost host = new RubyConsoleHost(new FakeConsoleTextEditor(), new FakeControlDispatcher());
			host.Dispose();
		}
		
		[Test]
		public void DefaultOutputStreamReplacedByCustomStreamClass()
		{
			host.CallCreateConsole(null, null, null);
			Assert.IsNotNull(host.OutputStream);
		}
		
		[Test]
		public void ProviderIsRubyContext()
		{
			Assert.AreEqual(typeof(RubyContext), host.GetProvider());
		}
	}
}
