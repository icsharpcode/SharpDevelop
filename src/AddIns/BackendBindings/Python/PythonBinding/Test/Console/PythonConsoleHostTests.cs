// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.PythonBinding;
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
		PythonConsoleTextEditor textEditor;
		
		[TestFixtureSetUp]
		public void Init()
		{
			textEditorControl = new TextEditor();
			textEditor = new PythonConsoleTextEditor(textEditorControl);
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
		/// When the console is disposed calling ReadLine returns null.
		/// </summary>
		[Test]
		public void HostDisposesPythonConsole()
		{
			DerivedPythonConsoleHost host = new DerivedPythonConsoleHost(new MockConsoleTextEditor());
			PythonConsole console = host.CallCreateConsole(null, null, null) as PythonConsole;
			host.Dispose();

			Assert.IsNull(console.ReadLine(0));
		}
		
		/// <summary>
		/// Makes sure the Dispose method checks if the python console is null before trying to dispose it.
		/// </summary>
		[Test]
		public void DisposingPythonConsoleHostWithoutCreatingPythonConsole()
		{
			PythonConsoleHost host = new PythonConsoleHost(new MockConsoleTextEditor());
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