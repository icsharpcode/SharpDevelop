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
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;

namespace ICSharpCode.Scripting
{
	public class RunScriptingConsoleApplicationCommand : AbstractMenuCommand
	{
		IDebuggerService debugger;
		IScriptingWorkbench workbench;
		ScriptingConsoleApplication scriptingConsoleApplication;
		
		public RunScriptingConsoleApplicationCommand(IScriptingWorkbench workbench, 
			IDebuggerService debugger,
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
