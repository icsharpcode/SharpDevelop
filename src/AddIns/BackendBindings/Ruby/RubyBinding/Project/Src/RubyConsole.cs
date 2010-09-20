// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Scripting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsole : ThreadSafeScriptingConsole, IConsole, IMemberProvider
	{
		IScriptingConsoleTextEditor textEditor;
		IControlDispatcher dispatcher;
		
		public RubyConsole(IScriptingConsoleTextEditor textEditor, IControlDispatcher dispatcher)
			: this(new ScriptingConsole(textEditor), dispatcher)
		{
			this.textEditor = textEditor;
		}
		
		RubyConsole(ScriptingConsole console, IControlDispatcher dispatcher)
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
		
		public void Write(string text, Style style)
		{
			base.Write(text, (ScriptingStyle)style);
		}
		
		public void WriteLine(string text, Style style)
		{
			base.WriteLine(text, (ScriptingStyle)style);
		}
		
		public IList<string> GetMemberNames(string name)
		{
			return CommandLine.GetMemberNames(name);
		}
		
		public IList<string> GetGlobals(string name)
		{
			return CommandLine.GetGlobals(name);
		}
	}
}
