// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using System.Threading;

using ICSharpCode.Scripting;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsoleHost : ConsoleHost, IScriptingConsoleHost
	{
		Thread thread;
		PythonConsole pythonConsole;
		
		public PythonConsoleHost(IScriptingConsoleTextEditor textEditor, IControlDispatcher dispatcher)
		{
			pythonConsole = new PythonConsole(textEditor, dispatcher);
		}
				
		public IScriptingConsole ScriptingConsole {
			get { return pythonConsole; }
		}
		
		protected override Type Provider {
			get { return typeof(PythonContext); }
		}
		
		/// <summary>
		/// Runs the console host in its own thread.
		/// </summary>
		public void Run()
		{
			thread = new Thread(RunConsole);
			thread.Start();
		}
		
		public void Dispose()
		{
			if (pythonConsole != null) {
				pythonConsole.Dispose();
			}
			
			if (thread != null) {
				thread.Join();
			}			
		}
				
		protected override CommandLine CreateCommandLine()
		{
			return new PythonCommandLine();
		}
		
		protected override OptionsParser CreateOptionsParser()
		{
			return new PythonOptionsParser();
		}
		
		/// <remarks>
		/// After the engine is created the standard output is replaced with our custom Stream class so we
		/// can redirect the stdout to the text editor window.
		/// This can be done in this method since the Runtime object will have been created before this method
		/// is called.
		/// </remarks>
		protected override IConsole CreateConsole(ScriptEngine engine, CommandLine commandLine, ConsoleOptions options)
		{
			ScriptingConsoleOutputStream stream = pythonConsole.CreateOutputStream();
			SetOutput(stream);
			pythonConsole.CommandLine = commandLine;
			return pythonConsole;
		}
		
		protected virtual void SetOutput(ScriptingConsoleOutputStream stream)
		{
			Runtime.IO.SetOutput(stream, Encoding.UTF8);
		}
		
		void RunConsole()
		{
			Run(new string[0]);
		}
	}
}
