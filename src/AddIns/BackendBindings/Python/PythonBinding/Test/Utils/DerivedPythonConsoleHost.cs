// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace PythonBinding.Tests.Console
{
	public class DerivedPythonConsoleHost : PythonConsoleHost
	{
		PythonOutputStream outputStream;
		
		public DerivedPythonConsoleHost(IScriptingConsoleTextEditor textEditor) : base(textEditor)
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
				
		/// <summary>
		/// Gets the output stream class passed to SetOutput method.
		/// </summary>
		public PythonOutputStream OutputStream {
			get { return outputStream; }
		}
			
		protected override void SetOutput(PythonOutputStream stream)
		{
			outputStream = stream;
		}
	}
}
