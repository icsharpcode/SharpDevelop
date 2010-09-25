// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Runs the Ruby console passing the filename of the 
	/// active Ruby script open in SharpDevelop.
	/// </summary>
	public class RunRubyCommand : AbstractMenuCommand
	{
		IDebugger debugger;
		RubyAddInOptions options;
		IScriptingWorkbench workbench;
		RubyConsoleApplication rubyConsoleApplication;
		
		public RunRubyCommand()
			: this(new RubyWorkbench(), new RubyAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunRubyCommand(IScriptingWorkbench workbench, RubyAddInOptions options, IDebugger debugger)
		{
			this.workbench = workbench;
			this.debugger = debugger;
			this.options = options;
			rubyConsoleApplication = new RubyConsoleApplication(options);
		}
		
		public bool Debug {
			get { return rubyConsoleApplication.Debug; }
			set { rubyConsoleApplication.Debug = value; }
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
			rubyConsoleApplication.ScriptFileName = GetRubyScriptFileName();
			rubyConsoleApplication.WorkingDirectory = GetWorkingDirectory();
			return rubyConsoleApplication.GetProcessStartInfo();
		}
		
		string GetWorkingDirectory()
		{
			return Path.GetDirectoryName(WorkbenchPrimaryFileName);
		}
		
		FileName WorkbenchPrimaryFileName {
			get { return workbench.ActiveViewContent.PrimaryFileName; }
		}
		
		string GetRubyScriptFileName()
		{
			return Path.GetFileName(WorkbenchPrimaryFileName);
		}
	}
}
