// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Debugging;

using Debugger;

namespace ICSharpCode.SharpDevelop.Services
{
	public class RunToCursorCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopTextAreaControl textEditor = this.Owner as SharpDevelopTextAreaControl;
			WindowsDebugger winDebugger = DebuggerService.CurrentDebugger as WindowsDebugger;
			if (textEditor == null || winDebugger == null) return;
			
			Breakpoint breakpoint = winDebugger.DebuggerCore.AddBreakpoint(textEditor.FileName, null, textEditor.ActiveTextAreaControl.Caret.Line + 1, textEditor.ActiveTextAreaControl.Caret.Column, true);
			breakpoint.Hit += delegate { breakpoint.Remove(); };
			winDebugger.DebuggedProcess.Paused += delegate { breakpoint.Remove(); };
			if (!winDebugger.IsProcessRunning) {
				winDebugger.Continue();
			}
		}
	}
}