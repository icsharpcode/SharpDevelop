// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
