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
using System.Text;
using System.Threading;

using ICSharpCode.Scripting;
using IronRuby;
using IronRuby.Hosting;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsoleHost : ConsoleHost, IScriptingConsoleHost
	{
		Thread thread;
		IScriptingConsoleTextEditor textEditor;
		RubyConsole rubyConsole;

		public RubyConsoleHost(IScriptingConsoleTextEditor textEditor, IControlDispatcher dispatcher)
		{
			this.textEditor = textEditor;
			rubyConsole = new RubyConsole(textEditor, dispatcher);
		}
		
		public IScriptingConsole ScriptingConsole {
			get { return rubyConsole; }
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
			if (rubyConsole != null) {
				rubyConsole.Dispose();
			}
			
			if (thread != null) {
				thread.Join();
			}			
		}

		void RunConsole()
		{
			Run(new string[0]);
		}
		
		protected virtual void SetOutput(ScriptingConsoleOutputStream stream)
		{
			Runtime.IO.SetOutput(stream, Encoding.UTF8);
		}
		
		protected override OptionsParser CreateOptionsParser()
		{
			return new RubyOptionsParser();
		}
		
		protected override CommandLine CreateCommandLine()
		{
			return new RubyCommandLine();
		}
		
		protected override LanguageSetup CreateLanguageSetup()
		{
			return Ruby.CreateRubySetup();
		}
		
		protected override Type Provider {
			get { return typeof(RubyContext); }
		}
		
		/// <remarks>
		/// After the engine is created the standard output is replaced with our custom Stream class so we
		/// can redirect the stdout to the text editor window.
		/// This can be done in this method since the Runtime object will have been created before this method
		/// is called.
		/// </remarks>
		protected override IConsole CreateConsole(ScriptEngine engine, CommandLine commandLine, ConsoleOptions options)
		{
			ScriptingConsoleOutputStream stream = rubyConsole.CreateOutputStream();
			SetOutput(stream);
			rubyConsole.CommandLine = commandLine;
			return rubyConsole;
		}
	}
}
