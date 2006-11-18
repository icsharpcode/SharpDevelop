// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class Execute : AbstractMenuCommand
	{
		protected bool withDebugger = true;
		
		public override void Run()
		{
			Build build = new Build();
			build.BuildComplete += delegate {
				if (build.LastBuildResults.ErrorCount == 0) {
					IProject startupProject = ProjectService.OpenSolution.StartupProject;
					if (startupProject != null) {
						startupProject.Start(withDebugger);
					} else {
						MessageService.ShowError("${res:BackendBindings.ExecutionManager.CantExecuteDLLError}");
					}
				}
			};
			build.Run();
		}
	}
	public class ExecuteWithoutDebugger : Execute
	{
		public override void Run()
		{
			withDebugger = false;
			base.Run();
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
	
	public class ToggleBreakpointCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			
			if (window == null || !(window.ViewContent is ITextEditorControlProvider)) {
				return;
			}
			TextEditorControl textEditor = ((ITextEditorControlProvider)window.ViewContent).TextEditorControl;
			DebuggerService.ToggleBreakpointAt(textEditor.Document, textEditor.FileName, textEditor.ActiveTextAreaControl.Caret.Line);
		}
	}
}
