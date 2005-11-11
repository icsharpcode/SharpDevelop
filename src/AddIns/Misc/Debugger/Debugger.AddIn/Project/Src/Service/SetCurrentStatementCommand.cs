/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 11.11.2005
 * Time: 23:48
 */

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

namespace ICSharpCode.SharpDevelop.Services
{
	public class SetCurrentStatementCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			SharpDevelopTextAreaControl textEditor = this.Owner as SharpDevelopTextAreaControl;
			if (textEditor == null) return;
			
			WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
			string fileName = textEditor.FileName;
			int line = textEditor.ActiveTextAreaControl.Caret.Line + 1;
			int column = textEditor.ActiveTextAreaControl.Caret.Column;
			
			debugger.DebuggerCore.CurrentThread.CurrentFunction.SetIP(fileName, line, column);
		}
	}
}
