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
