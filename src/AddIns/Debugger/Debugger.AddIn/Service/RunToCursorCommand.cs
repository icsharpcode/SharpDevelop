// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using Debugger;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class RunToCursorCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			WindowsDebugger winDebugger = DebuggerService.CurrentDebugger as WindowsDebugger;
			
			if (provider == null || winDebugger == null)
				return;
			
			ITextEditor textEditor = provider.TextEditor;
			
			Breakpoint breakpoint = winDebugger.DebuggerCore.Breakpoints.Add(textEditor.FileName, null, textEditor.Caret.Line, textEditor.Caret.Column, true);
			// Be careful to remove the breakpoint just once
			breakpoint.Hit += delegate {
				if (breakpoint != null)
					breakpoint.Remove();
				breakpoint = null;
			};
			winDebugger.DebuggedProcess.Paused += delegate {
				if (breakpoint != null)
					breakpoint.Remove();
				breakpoint = null;
			};
			if (!winDebugger.IsProcessRunning) {
				winDebugger.Continue();
			}
		}
	}
}
