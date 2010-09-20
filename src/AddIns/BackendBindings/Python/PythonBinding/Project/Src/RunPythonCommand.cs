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

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Runs the Python console passing the filename of the 
	/// active python script open in SharpDevelop.
	/// </summary>
	public class RunPythonCommand : AbstractMenuCommand
	{
		IDebugger debugger;
		PythonAddInOptions options;
		IScriptingWorkbench workbench;
		PythonConsoleApplication ipy;
		
		public RunPythonCommand()
			: this(new PythonWorkbench(), new PythonAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunPythonCommand(IScriptingWorkbench workbench, PythonAddInOptions options, IDebugger debugger)
		{
			this.workbench = workbench;
			this.debugger = debugger;
			this.options = options;
			ipy = new PythonConsoleApplication(options);
		}
		
		public bool Debug {
			get { return ipy.Debug; }
			set { ipy.Debug = value; }
		}
		
		public override void Run()
		{
			ProcessStartInfo processStartInfo = GetProcessStartInfo();
			if (Debug) {
				debugger.Start(processStartInfo);
			} else {
				debugger.StartWithoutDebugging(processStartInfo);
			}
		}
		
		ProcessStartInfo GetProcessStartInfo()
		{
			string scriptFileName = workbench.ActiveViewContent.PrimaryFileName;
			ipy.PythonScriptFileName = scriptFileName;
			ipy.WorkingDirectory = Path.GetDirectoryName(scriptFileName);
			return ipy.GetProcessStartInfo();
		}
	}
}
