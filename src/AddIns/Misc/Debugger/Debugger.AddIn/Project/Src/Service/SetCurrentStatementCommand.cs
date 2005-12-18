// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
