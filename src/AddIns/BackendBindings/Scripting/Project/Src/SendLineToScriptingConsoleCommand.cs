// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public class SendLineToScriptingConsoleCommand : AbstractCommand
	{
		IScriptingWorkbench workbench;
		
		IScriptingConsolePad consolePad;
		ScriptingTextEditorViewContent textEditorView;
		ITextEditor activeTextEditor;
		IScriptingConsole pythonConsole;
		string lineFromActiveTextEditor;
		
		public SendLineToScriptingConsoleCommand(IScriptingWorkbench workbench)
		{
			this.workbench = workbench;
			
			textEditorView = new ScriptingTextEditorViewContent(workbench);
			activeTextEditor = textEditorView.TextEditor;
		}	
		
		public override void Run()
		{
			GetLineFromActiveTextEditor();
			GetScriptingConsolePad();
			ShowScriptingConsolePad();
			AppendLineToScriptingConsole();
		}
		
		void GetLineFromActiveTextEditor()
		{
			int lineNumber = activeTextEditor.Caret.Line;
			IDocumentLine documentLine =  activeTextEditor.Document.GetLine(lineNumber);
			lineFromActiveTextEditor = documentLine.Text;
		}
		
		void GetScriptingConsolePad()
		{
			consolePad = workbench.GetScriptingConsolePad();
		}
				
		void ShowScriptingConsolePad()
		{
			consolePad.BringToFront();
		}
		
		void AppendLineToScriptingConsole()
		{
			GetScriptingConsole();
			pythonConsole.SendLine(lineFromActiveTextEditor);
		}
		
		void GetScriptingConsole()
		{
			pythonConsole = consolePad.ScriptingConsole;
		}
	}
}
