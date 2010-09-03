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

namespace ICSharpCode.PythonBinding
{
	public class SendLineToPythonConsoleCommand : AbstractCommand
	{
		IScriptingWorkbench workbench;
		IScriptingConsolePad consolePad;
		ScriptingTextEditorViewContent textEditorView;
		ITextEditor activeTextEditor;
		IScriptingConsole pythonConsole;
		string lineFromActiveTextEditor;
		
		public SendLineToPythonConsoleCommand()
			: this(new PythonWorkbench())
		{
		}
		
		public SendLineToPythonConsoleCommand(IScriptingWorkbench workbench)
		{
			this.workbench = workbench;
			
			textEditorView = new ScriptingTextEditorViewContent(workbench);
			activeTextEditor = textEditorView.TextEditor;
		}	
		
		public override void Run()
		{
			GetLineFromActiveTextEditor();
			GetPythonConsolePad();
			ShowPythonConsolePad();
			AppendLineToPythonConsole();
		}
		
		void GetLineFromActiveTextEditor()
		{
			int lineNumber = activeTextEditor.Caret.Line;
			IDocumentLine documentLine =  activeTextEditor.Document.GetLine(lineNumber);
			lineFromActiveTextEditor = documentLine.Text;
		}
		
		void GetPythonConsolePad()
		{
			consolePad = workbench.GetScriptingConsolePad();
		}
				
		void ShowPythonConsolePad()
		{
			consolePad.BringToFront();
		}
		
		void AppendLineToPythonConsole()
		{
			GetPythonConsole();
			pythonConsole.SendLine(lineFromActiveTextEditor);
		}
		
		void GetPythonConsole()
		{
			pythonConsole = consolePad.ScriptingConsole;
		}
	}
}
