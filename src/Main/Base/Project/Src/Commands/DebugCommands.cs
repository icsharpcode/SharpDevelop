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
			DebuggerService.CurrentDebugger.StepOver();
		}
	}
	
	public class StepIntoDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			LoggingService.Info("Debugger Command: StepInto");
			DebuggerService.CurrentDebugger.StepInto();
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
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveContent as ITextEditorProvider;
			
			if (provider != null) {
				ITextEditor editor = provider.TextEditor;
				if (!string.IsNullOrEmpty(editor.FileName)) {
					DebuggerService.ToggleBreakpointAt(editor, editor.Caret.Line);
				}
			}
		}
	}
	
	public class RemoveAllBreakpointsCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (DebuggerService.Breakpoints.Count <= 0) return;
			
			if(System.Windows.Forms.MessageBox.Show(
				StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.Debug.RemoveAllBreakPoints}"),
				StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.Debug.RemoveAllBreakPointsCaption}"),
				System.Windows.Forms.MessageBoxButtons.YesNo, 
				System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
			{
				BookmarkManager.RemoveAll(b => b is BreakpointBookmark);
			}
		}
	}
	
	public abstract class NextPrevBreakpointCommand : AbstractMenuCommand
	{
		public void Run(ListViewPadItemModel nextItem)
		{
			var bookmarkBase = (BookmarkPadBase)Owner;	
			
			if (nextItem == null) return;
			
			// get next bookmark						
			int line = (nextItem.Mark as SDBookmark).LineNumber;
			
			var bookmarks = DebuggerService.Breakpoints;
			var bookmark = bookmarks.FirstOrDefault(b => b.LineNumber == line);
			if (bookmark == null && bookmarks.Count > 0) {
				bookmark = bookmarks[0]; // jump around to first bookmark
			}
			if (bookmark != null) {
				FileService.JumpToFilePosition(bookmark.FileName, bookmark.LineNumber, bookmark.ColumnNumber);
			}	

			// select in tree
			bookmarkBase.SelectItem(nextItem);
		}
	}
	
	public sealed class NextBreakpointCommand : NextPrevBreakpointCommand
	{
		public override void Run()
		{
			var bookmarkBase = (BookmarkPadBase)Owner;			
			var nextItem = bookmarkBase.NextItem;
			
			base.Run(nextItem);
		}
	}
	
	public sealed class PrevBreakpointCommand : NextPrevBreakpointCommand
	{
		public override void Run()
		{
			var bookmarkBase = (BookmarkPadBase)Owner;			
			var prevItem = bookmarkBase.PrevItem;
			
			base.Run(prevItem);	
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
}
