// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ICSharpCode.Scripting;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsoleApplication : ScriptingConsoleApplication
	{
		RubyAddInOptions options;
		List<string> loadPaths = new List<string>();
		
		public RubyConsoleApplication(RubyAddInOptions options)
		{
			this.options = options;
		}
		
		public override string FileName {
			get { return options.RubyFileName; }
		}
		
		public void AddLoadPath(string path)
		{
			loadPaths.Add(path);
		}
		
		protected override void AddArguments(ScriptingCommandLineBuilder commandLine)
		{
			commandLine.AppendOption("--disable-gems");
			commandLine.AppendBooleanOptionIfTrue("-D", Debug);
			AppendLoadPaths(commandLine);
			commandLine.AppendQuotedStringIfNotEmpty(ScriptFileName);
			commandLine.AppendStringIfNotEmpty(ScriptCommandLineArguments);
		}
		
		void AppendLoadPaths(ScriptingCommandLineBuilder commandLine)
		{
			foreach (string path in loadPaths) {
				commandLine.AppendQuotedString("-I" + path);
			}
		}
	}
}
