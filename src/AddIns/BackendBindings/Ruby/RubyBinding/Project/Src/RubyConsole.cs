// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Scripting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsole : ScriptingConsole, IConsole
	{
		public RubyConsole(IScriptingConsoleTextEditor textEditor)
			: base(textEditor)
		{
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
	}
}
