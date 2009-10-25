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
			breakpoint.Hit += delegate { breakpoint.Remove(); };
			winDebugger.DebuggedProcess.Paused += delegate { breakpoint.Remove(); };
			if (!winDebugger.IsProcessRunning) {
				winDebugger.Continue();
			}
		}
	}
}