// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.Scripting
{
	public abstract class ScriptingConsoleApplication
	{
		public ScriptingConsoleApplication()
		{
			WorkingDirectory = String.Empty;
			ScriptFileName = String.Empty;
			ScriptCommandLineArguments = String.Empty;
		}
		
		public bool Debug { get; set; }
		
		/// <summary>
		/// Console application filename.
		/// </summary>
		public virtual string FileName { get; set; }
		
		public string WorkingDirectory { get; set; }
		
		public string ScriptFileName { get; set; }
		
		public string ScriptCommandLineArguments { get; set; }
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = FileName;
			processStartInfo.Arguments = GetArguments();
			processStartInfo.WorkingDirectory = WorkingDirectory;
			return processStartInfo;
		}
		
		public string GetArguments()
		{
			ScriptingCommandLineBuilder commandLine = new ScriptingCommandLineBuilder();
			AddArguments(commandLine);
			return commandLine.ToString();
		}
		
		protected virtual void AddArguments(ScriptingCommandLineBuilder commandLine)
		{
		}
	}
}
