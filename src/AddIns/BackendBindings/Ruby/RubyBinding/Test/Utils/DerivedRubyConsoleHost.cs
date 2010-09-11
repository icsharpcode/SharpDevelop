// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace RubyBinding.Tests.Console
{	
	public class DerivedRubyConsoleHost : RubyConsoleHost
	{
		ScriptingConsoleOutputStream outputStream;
		
		public DerivedRubyConsoleHost(IScriptingConsoleTextEditor textEditor)
			: base(textEditor, new FakeControlDispatcher())
		{
		}
		
		public Type GetProvider()
		{
			return base.Provider;
		}
		
		public CommandLine CallCreateCommandLine()
		{
			return base.CreateCommandLine();
		}
		
		public IConsole CallCreateConsole(ScriptEngine engine, CommandLine commandLine, ConsoleOptions options)
		{
			return base.CreateConsole(engine, commandLine, options);
		}
		
		public OptionsParser CallCreateOptionsParser()
		{
			return base.CreateOptionsParser();
		}
		
		public LanguageSetup CallCreateLanguageSetup()
		{
			return base.CreateLanguageSetup();
		}
				
		/// <summary>
		/// Gets the output stream class passed to SetOutput method.
		/// </summary>
		public ScriptingConsoleOutputStream OutputStream {
			get { return outputStream; }
		}
			
		protected override void SetOutput(ScriptingConsoleOutputStream stream)
		{
			outputStream = stream;
		}
	}
}
