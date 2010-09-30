// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Text;
using ICSharpCode.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsoleApplication : ScriptingConsoleApplication
	{
		PythonAddInOptions options;
		
		public PythonConsoleApplication(PythonAddInOptions options)
		{
			this.options = options;
		}
		
		public override string FileName {
			get { return options.PythonFileName; }
		}
				
		protected override void AddArguments(ScriptingCommandLineBuilder commandLine)
		{
			commandLine.AppendBooleanOptionIfTrue("-X:Debug", Debug);
			commandLine.AppendQuotedStringIfNotEmpty(ScriptFileName);
			commandLine.AppendStringIfNotEmpty(ScriptCommandLineArguments);
		}
	}
}
