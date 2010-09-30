// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.Scripting
{
	public class RunScriptingConsoleApplicationCommand : AbstractMenuCommand
	{
		IDebugger debugger;
		IScriptingWorkbench workbench;
		ScriptingConsoleApplication scriptingConsoleApplication;
		
		public RunScriptingConsoleApplicationCommand(IScriptingWorkbench workbench, 
			IDebugger debugger,
			ScriptingConsoleApplication scriptingConsoleApplication)
		{
			this.workbench = workbench;
			this.debugger = debugger;
			this.scriptingConsoleApplication = scriptingConsoleApplication;
		}
		
		public bool Debug {
			get { return scriptingConsoleApplication.Debug; }
			set { scriptingConsoleApplication.Debug = value; }
		}
		
		public override void Run()
		{
			ProcessStartInfo processStartInfo = CreateProcessStartInfo();
			if (Debug) {
				debugger.Start(processStartInfo);
			} else {
				PauseCommandPromptProcessStartInfo commandPrompt = new PauseCommandPromptProcessStartInfo(processStartInfo);
				debugger.StartWithoutDebugging(commandPrompt.ProcessStartInfo);
			}
		}
		
		ProcessStartInfo CreateProcessStartInfo()
		{
			scriptingConsoleApplication.ScriptFileName = GetScriptFileName();
			scriptingConsoleApplication.WorkingDirectory = GetWorkingDirectory();
			return scriptingConsoleApplication.GetProcessStartInfo();
		}
		
		string GetWorkingDirectory()
		{
			return Path.GetDirectoryName(WorkbenchPrimaryFileName);
		}
		
		FileName WorkbenchPrimaryFileName {
			get { return workbench.ActiveViewContent.PrimaryFileName; }
		}
		
		string GetScriptFileName()
		{
			return Path.GetFileName(WorkbenchPrimaryFileName);
		}
	}
}
