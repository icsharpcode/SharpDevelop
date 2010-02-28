// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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