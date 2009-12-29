// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.TextEditor;
using ICSharpCode.RubyBinding;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Hosting.Shell;

namespace RubyBinding.Tests.Console
{
	public class DerivedRubyConsoleHost : RubyConsoleHost
	{
		RubyOutputStream outputStream;
		
		public DerivedRubyConsoleHost(ITextEditor textEditor) : base(textEditor)
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
		public RubyOutputStream OutputStream {
			get { return outputStream; }
		}
			
		protected override void SetOutput(RubyOutputStream stream)
		{
			outputStream = stream;
		}
	}
}
