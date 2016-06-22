// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
using Debugger.AddIn.Breakpoints;
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
			
			if (textEditor == null || SD.Debugger == null)
				return;
			
			SD.Debugger.SetInstructionPointer(textEditor.FileName, textEditor.Caret.Line, textEditor.Caret.Column, false);
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
			SD.Debugger.BreakAtBeginning = DebuggingOptions.Instance.BreakAtBeginning;
			SD.Debugger.Start(new ProcessStartInfo {
			                      	FileName = fileName,
			                      	WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(fileName),
			                      	Arguments = arguments
			                      });
		}
	}
}
