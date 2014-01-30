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
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Basic tests for the PythonConsoleHost class.
	/// </summary>
	[TestFixture]
	public class PythonConsoleHostTests
	{
		DerivedPythonConsoleHost host;
		TextEditor textEditorControl;
		ScriptingConsoleTextEditor textEditor;
		
		[TestFixtureSetUp]
		public void Init()
		{
			textEditorControl = new TextEditor();
			textEditor = new ScriptingConsoleTextEditor(textEditorControl);
			host = new DerivedPythonConsoleHost(textEditor);
			
			ScriptRuntime runtime = IronPython.Hosting.Python.CreateRuntime();
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			host.Dispose();
		}
		
		[Test]
		public void PythonConsoleHostIsMicrosoftScriptingConsoleHostType()
		{
			Assert.IsInstanceOf(typeof(ConsoleHost), host);
		}
		
		[Test]
		public void OptionsParserCreatedIsPythonOptionsParser()
		{
			OptionsParser parser = host.CallCreateOptionsParser();
			Assert.IsInstanceOf(typeof(PythonOptionsParser), parser);
		}
		
		[Test]
		public void CommandLineCreatedIsPythonCommandLine()
		{
			CommandLine commandLine = host.CallCreateCommandLine();
			Assert.IsInstanceOf(typeof(PythonCommandLine), commandLine);
		}

		[Test]
		public void ConsoleCreatedIsPythonConsole()
		{
			IConsole console = host.CallCreateConsole(null, null, null);
			Assert.IsInstanceOf(typeof(PythonConsole), console);
		}
		
		[Test]
		public void PythonContextIsProvider()
		{
			Assert.AreEqual(typeof(PythonContext), host.GetProvider());
		}
		
		[Test]
		public void ConsoleHostImplementsIDisposable()
		{
			Assert.IsNotNull(host as IDisposable);
		}
		
		/// <summary>
		/// Makes sure the Dispose method checks if the python console is null before trying to dispose it.
		/// </summary>
		[Test]
		public void DisposingPythonConsoleHostWithoutCreatingPythonConsole()
		{
			PythonConsoleHost host = new PythonConsoleHost(new FakeConsoleTextEditor(), new FakeControlDispatcher());
			host.Dispose();
		}
		
		[Test]
		public void DefaultOutputStreamReplacedByCustomStreamClass()
		{
			host.CallCreateConsole(null, null, null);
			Assert.IsNotNull(host.OutputStream);
		}
	}
}
