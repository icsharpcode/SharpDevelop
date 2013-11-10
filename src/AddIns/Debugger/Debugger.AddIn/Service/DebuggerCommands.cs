// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using Microsoft.Win32;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn
{
	public class RunToCursorCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
			if (editor == null || WindowsDebugger.CurrentProcess == null)
				return;
			WindowsDebugger.CurrentProcess.RunTo(editor.FileName, editor.Caret.Line, editor.Caret.Column);
		}
	}
	
	public class SetCurrentStatementCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditor textEditor = SD.GetActiveViewContentService<ITextEditor>();
			
			if (textEditor == null || DebuggerService.CurrentDebugger == null)
				return;
			
			DebuggerService.CurrentDebugger.SetInstructionPointer(textEditor.FileName, textEditor.Caret.Line, textEditor.Caret.Column, false);
		}
	}
	
	public static class BreakpointUtil
	{
		public static IEnumerable<BreakpointBookmark> BreakpointsOnCaret {
			get {
				ITextEditor editor = SD.GetActiveViewContentService<ITextEditor>();
				if (editor == null)
					return new BreakpointBookmark[0];
				
				return SD.BookmarkManager.Bookmarks.OfType<BreakpointBookmark>().Where(bp => bp.FileName == editor.FileName && bp.LineNumber == editor.Caret.Line);
			}
		}
	}
	
	public class EnableBreakpointMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (BreakpointBookmark bp in BreakpointUtil.BreakpointsOnCaret) {
				bp.IsEnabled = true;
			}
		}
	}
	
	public class DisableBreakpointMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (BreakpointBookmark bp in BreakpointUtil.BreakpointsOnCaret) {
				bp.IsEnabled = false;
			}
		}
	}
	
	public class EditBreakpointMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			foreach (BreakpointBookmark bp in BreakpointUtil.BreakpointsOnCaret) {
				EditBreakpointScriptWindow window = new EditBreakpointScriptWindow(bp) {
					Owner = SD.Workbench.MainWindow
				};
				window.ShowDialog();
			}
		}
	}
	
	public class IsActiveBreakpointCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return BreakpointUtil.BreakpointsOnCaret.Any(bp => bp.IsEnabled);
		}
	}
	
	public class IsBreakpointCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			return BreakpointUtil.BreakpointsOnCaret.Any();
		}
	}
	
	public class DebugExecutableMenuCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (DebuggingOptions.Instance.AskForArguments) {
				var window = new ExecuteProcessWindow { Owner = SD.Workbench.MainWindow };
				if (window.ShowDialog() == true) {
					string fileName = window.SelectedExecutable;
					
					// execute the process
					StartExecutable(fileName, window.WorkingDirectory, window.Arguments);
				}
			} else {
				OpenFileDialog dialog = new OpenFileDialog() {
					Filter = ".NET executable|*.exe",
					RestoreDirectory = true,
					DefaultExt = "exe"
				};
				if (dialog.ShowDialog() == true) {
					string fileName = dialog.FileName;
					// execute the process
					StartExecutable(fileName);
				}
			}
		}
		
		void StartExecutable(string fileName, string workingDirectory = null, string arguments = null)
		{
			DebuggerService.CurrentDebugger.BreakAtBeginning = DebuggingOptions.Instance.BreakAtBeginning;
			DebuggerService.CurrentDebugger.Start(new ProcessStartInfo {
			                      	FileName = fileName,
			                      	WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(fileName),
			                      	Arguments = arguments
			                      });
		}
	}
}
