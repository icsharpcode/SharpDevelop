// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
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
		AddInOptions options;
		IWorkbench workbench;
		bool debug;
		
		public RunPythonCommand()
			: this(WorkbenchSingleton.Workbench, new AddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunPythonCommand(IWorkbench workbench, AddInOptions options, IDebugger debugger)
		{
			this.workbench = workbench;
			this.debugger = debugger;
			this.options = options;
		}
		
		public bool Debug {
			get { return debug; }
			set { debug = value; }
		}
		
		public override void Run()
		{
			if (debug) {
				debugger.Start(CreateProcessStartInfo());
			} else {
				debugger.StartWithoutDebugging(CreateProcessStartInfo());
			}
		}
		
		ProcessStartInfo CreateProcessStartInfo()
		{
			ProcessStartInfo info = new ProcessStartInfo();
			info.FileName = options.PythonFileName;
			info.Arguments = GetArguments();
				
			return info;
		}
		
		string GetArguments()
		{
			// Get the python script filename.
			string pythonScriptFileName = "\"" + workbench.ActiveWorkbenchWindow.ActiveViewContent.PrimaryFileName + "\"";
			
			if (Debug) {
				return "-D " + pythonScriptFileName;
			}
			return pythonScriptFileName;
		}
	}
}
