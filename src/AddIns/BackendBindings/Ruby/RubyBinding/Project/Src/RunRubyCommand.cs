// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;
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
		IWorkbench workbench;
		bool debug;
		
		public RunRubyCommand()
			: this(WorkbenchSingleton.Workbench, new RubyAddInOptions(), DebuggerService.CurrentDebugger)
		{
		}
		
		public RunRubyCommand(IWorkbench workbench, RubyAddInOptions options, IDebugger debugger)
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
			info.FileName = options.RubyFileName;
			info.Arguments = GetArguments();
			info.WorkingDirectory = Path.GetDirectoryName(workbench.ActiveWorkbenchWindow.ActiveViewContent.PrimaryFileName);
				
			return info;
		}
		
		string GetArguments()
		{
			// Get the Ruby script filename.
			string rubyScriptFileName = Path.GetFileName(workbench.ActiveWorkbenchWindow.ActiveViewContent.PrimaryFileName);
			string args = "--disable-gems " + rubyScriptFileName;
			
			if (Debug) {
				return "-D " + args;
			}
			return args;
		}
	}
}
