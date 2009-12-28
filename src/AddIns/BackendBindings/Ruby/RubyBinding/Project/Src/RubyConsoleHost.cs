// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Threading;
using IronRuby;
using IronRuby.Hosting;
using IronRuby.Runtime;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsoleHost : ConsoleHost, IDisposable
	{
		Thread thread;
		ITextEditor textEditor;
		RubyConsole rubyConsole;

		public RubyConsoleHost(ITextEditor textEditor)
		{
			this.textEditor = textEditor;
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

		/// <summary>
		/// Runs the console.
		/// </summary>
		void RunConsole()
		{
			Run(new string[0]);
		}
		
		protected virtual void SetOutput(RubyOutputStream stream)
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
			SetOutput(new RubyOutputStream(textEditor));
			rubyConsole = new RubyConsole(textEditor, commandLine);
			return rubyConsole;
		}
	}
}
