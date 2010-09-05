// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Scripting;
using Microsoft.Scripting.Hosting.Shell;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsole : ScriptingConsole, IConsole
	{
		public PythonConsole(IScriptingConsoleTextEditor textEditor)
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
		
		/// <summary>
		/// Gets the member names of the specified item.
		/// </summary>
		public override IList<string> GetMemberNames(string name)
		{
			return CommandLine.GetMemberNames(name);
		}
		
		public override IList<string> GetGlobals(string name)
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
