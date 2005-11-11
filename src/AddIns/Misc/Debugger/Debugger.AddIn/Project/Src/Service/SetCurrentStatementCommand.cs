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
			if (textEditor == null || DebuggerService.CurrentDebugger == null) return;
			
			DebuggerService.CurrentDebugger.SetInstructionPointer(textEditor.FileName, textEditor.ActiveTextAreaControl.Caret.Line + 1, textEditor.ActiveTextAreaControl.Caret.Column);
		}
	}
}
