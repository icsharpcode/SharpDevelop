// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
			Debug.Assert(ProjectService.OpenSolution != null);
//			if (ProjectService.OpenSolution.IsDirty) {
//				new 
//			}
			new Build().Run();
			IProject startupProject = ProjectService.OpenSolution.StartupProject;
			if (startupProject != null) {
				startupProject.Start(true);
			}
		}
	}
	public class ExecuteWithoutDebugger : AbstractMenuCommand
	{
		public override void Run()
		{
			Debug.Assert(ProjectService.OpenSolution != null);
			
//			if (ProjectService.OpenSolution.IsDirty) {
//				ProjectService.OpenSolution.Build();
//			}
			new Build().Run();
			IProject startupProject = ProjectService.OpenSolution.StartupProject;
			if (startupProject != null) {
				startupProject.Start(false);
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
