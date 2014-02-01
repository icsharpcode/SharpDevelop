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
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Scripting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsole : ThreadSafeScriptingConsole, IConsole, IMemberProvider
	{
		IScriptingConsoleTextEditor textEditor;
		IControlDispatcher dispatcher;
		
		public PythonConsole(IScriptingConsoleTextEditor textEditor, IControlDispatcher dispatcher)
			: this(new ScriptingConsole(textEditor), dispatcher)
		{
			this.textEditor = textEditor;
		}
		
		PythonConsole(ScriptingConsole console, IControlDispatcher dispatcher)
			: base(console, dispatcher)
		{
			this.dispatcher = dispatcher;
			console.MemberProvider = this;
		}
		
		public ScriptingConsoleOutputStream CreateOutputStream()
		{
			return new ScriptingConsoleOutputStream(textEditor, dispatcher);
		}
		
		public CommandLine CommandLine { get; set; }
		
		public TextWriter Output {
			get { return null; }
			set { }
		}
		
		public TextWriter ErrorOutput {
			get { return null; }
			set { }
		}
		
		/// <summary>
		/// Gets the member names of the specified item.
		/// </summary>
		public IList<string> GetMemberNames(string name)
		{
			return CommandLine.GetMemberNames(name);
		}
		
		public IList<string> GetGlobals(string name)
		{
			return CommandLine.GetGlobals(name);
		}
		
		public void Write(string text, Style style)
		{
			base.Write(text, (ScriptingStyle)style);
		}
		
		public void WriteLine(string text, Style style)
		{
			base.WriteLine(text, (ScriptingStyle)style);
		}
	}
}
