// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.TextEditor;
using ICSharpCode.PythonBinding;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace PythonBinding.Tests.Console
{
	public class DerivedPythonConsoleHost : PythonConsoleHost
	{
		PythonOutputStream outputStream;
		
		public DerivedPythonConsoleHost(ITextEditor textEditor) : base(textEditor)
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
