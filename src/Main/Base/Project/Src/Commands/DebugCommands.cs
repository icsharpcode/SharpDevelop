// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class Execute : AbstractMenuCommand
	{
		public override void Run()
		{
			new Build().Run();
			if (Build.LastErrorCount == 0) {
				IProject startupProject = ProjectService.OpenSolution.StartupProject;
				if (startupProject != null) {
					startupProject.Start(true);
				} else {
					MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
				}
			}
		}
	}
	public class ExecuteWithoutDebugger : AbstractMenuCommand
	{
		public override void Run()
		{
			new Build().Run();
			if (Build.LastErrorCount == 0) {
				IProject startupProject = ProjectService.OpenSolution.StartupProject;
				if (startupProject != null) {
					startupProject.Start(false);
				} else {
					MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
				}
			}
		}
	}
	
	public class ContinueDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.Continue();
		}
	}
	
	public class BreakDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.Break();
		}
	}
	
	public class StopDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.Stop();
		}
	}
	
	public class StepDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.StepOver();
		}
	}
	
	public class StepIntoDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.StepInto();
		}
	}
	
	public class StepOutDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.StepOut();
		}
	}
}
