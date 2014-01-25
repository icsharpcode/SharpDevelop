// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
