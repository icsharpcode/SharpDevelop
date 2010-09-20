// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting
{
	public abstract class SendToScriptingConsoleCommand : AbstractMenuCommand
	{
		protected IScriptingWorkbench workbench;
		
		protected IScriptingConsolePad consolePad;
		protected ScriptingTextEditorViewContent textEditorView;
		protected ITextEditor activeTextEditor;
		protected IScriptingConsole scriptingConsole;
		
		public SendToScriptingConsoleCommand(IScriptingWorkbench workbench)
		{
			this.workbench = workbench;
			
			textEditorView = new ScriptingTextEditorViewContent(workbench);
			activeTextEditor = textEditorView.TextEditor;
		}
		
		protected void GetScriptingConsolePad()
		{
			consolePad = workbench.GetScriptingConsolePad();
		}
				
		protected void ShowScriptingConsolePad()
		{
			consolePad.BringToFront();
		}
		
		protected void GetScriptingConsole()
		{
			scriptingConsole = consolePad.ScriptingConsole;
		}
	}
}
