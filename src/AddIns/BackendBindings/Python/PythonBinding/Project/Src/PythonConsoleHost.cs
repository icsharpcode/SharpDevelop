// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Threading;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Hosts the python console.
	/// </summary>
	public class PythonConsoleHost : ConsoleHost, IDisposable
	{
		Thread thread;
		ITextEditor textEditor;
		PythonConsole pythonConsole;
				
		public PythonConsoleHost(ITextEditor textEditor)
		{
			this.textEditor = textEditor;
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
			SetOutput(new PythonOutputStream(textEditor));
			pythonConsole = new PythonConsole(textEditor, commandLine);
			return pythonConsole;
		}
		
		protected virtual void SetOutput(PythonOutputStream stream)
		{
			Runtime.IO.SetOutput(stream, Encoding.UTF8);
		}
		
		/// <summary>
		/// Runs the console.
		/// </summary>
		void RunConsole()
		{
			Run(new string[0]);
		}
	}
}
