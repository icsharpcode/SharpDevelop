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
			if (ProjectService.OpenSolution.IsDirty) {
				ProjectService.OpenSolution.Build();
			}
			ProjectService.OpenSolution.StartupProject.Start(true);
		}
	}
	public class ExecuteWithoutDebugger : AbstractMenuCommand
	{
		public override void Run()
		{
			Debug.Assert(ProjectService.OpenSolution != null);
			
			if (ProjectService.OpenSolution.IsDirty) {
				ProjectService.OpenSolution.Build();
			}
			ProjectService.OpenSolution.StartupProject.Start(false);
		}
	}
	
	public class ContinueDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.Continue();
		}
	}
	
	public class BreakDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.Break();
		}
	}
	
	public class StopDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.Stop();
		}
	}
	
	public class StepDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.Step(false);
		}
	}
	
	public class StepIntoDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.Step(true);
		}
	}
	
	public class StepOutDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.StepOut();
		}
	}
}
