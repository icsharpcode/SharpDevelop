// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Bookmarks;
using ICSharpCode.SharpDevelop.Bookmarks.Pad.Controls;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project.Commands
{
	public class Execute : AbstractMenuCommand
	{
		protected bool withDebugger = true;
		
		public override void Run()
		{
			Build build = new BuildBeforeExecute();
			build.BuildComplete += delegate {
				if (build.LastBuildResults.ErrorCount == 0) {
					IProject startupProject = ProjectService.OpenSolution.StartupProject;
					if (startupProject != null) {
						LoggingService.Info("Debugger Command: Start (withDebugger=" + withDebugger + ")");
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
			if (DebuggerService.CurrentDebugger.IsDebugging) {
				LoggingService.Info("Debugger Command: Continue");
				DebuggerService.CurrentDebugger.Continue();
			} else {
				LoggingService.Info("Debugger Command: Run");
				new Execute().Run();
			}
		}
	}
	
	public class BreakDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Debugger Command: Break");
			DebuggerService.CurrentDebugger.Break();
		}
	}
	
	public class StopDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Debugger Command: Stop");
			DebuggerService.CurrentDebugger.Stop();
		}
	}
	
	public class StepDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Debugger Command: StepOver");
			if (!DebuggerService.CurrentDebugger.IsDebugging) {
				DebuggerService.CurrentDebugger.BreakAtBeginning = true;
				new Execute().Run();
			} else {
				DebuggerService.CurrentDebugger.StepOver();
			}
		}
	}
	
	public class StepIntoDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Debugger Command: StepInto");
			if (!DebuggerService.CurrentDebugger.IsDebugging) {
				DebuggerService.CurrentDebugger.BreakAtBeginning = true;
				new Execute().Run();
			} else {
				DebuggerService.CurrentDebugger.StepInto();
			}
		}
	}
	
	public class StepOutDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Debugger Command: StepOut");
			DebuggerService.CurrentDebugger.StepOut();
		}
	}
	
	public class ToggleBreakpointCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			var viewContent = WorkbenchSingleton.Workbench.ActiveContent;
			ITextEditorProvider provider = viewContent as ITextEditorProvider;
			ITextEditor editor = null;
			
			if (provider != null) {
				editor = provider.TextEditor;
				if (!string.IsNullOrEmpty(editor.FileName)) {
					DebuggerService.ToggleBreakpointAt(editor, editor.Caret.Line, typeof(BreakpointBookmark));
				}
			} else {
				var view = viewContent as AbstractViewContentWithoutFile;
				if (view != null) {
					editor = view.GetService(typeof(ITextEditor)) as ITextEditor;
					if (editor != null) {
						DebuggerService.ToggleBreakpointAt(editor, editor.Caret.Line, typeof(DecompiledBreakpointBookmark));
					}
				}
			}
		}
	}
	
	public class RemoveAllBreakpointsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			BookmarkManager.RemoveAll(b => b is BreakpointBookmark);
		}
	}
	
	public class DeleteBreakpointCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (Owner == null || !(Owner is BookmarkPadBase)) return;
			
			var bookmarkBase = (BookmarkPadBase)Owner;
			var item = bookmarkBase.CurrentItem;
			
			if (item == null) return;
			
			if (item.Mark is BreakpointBookmark) {
				BookmarkManager.RemoveMark(item.Mark);
			}
		}
	}
	
	public class AttachToProcessCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.ShowAttachDialog();
		}
	}
	
	public class DetachFromProcessCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService.CurrentDebugger.Detach();
		}
	}
	
	public class ExecuteLastBuild : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ProjectService.OpenSolution == null)
				return;
			
			if (ProjectService.OpenSolution.StartupProject == null)
				return;
			
			ProjectService.OpenSolution.StartupProject.Start(false);
		}
	}
}
