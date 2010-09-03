// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PythonBinding
{
	public class SendLineToPythonConsoleCommand : AbstractCommand
	{
		IPythonWorkbench workbench;
		IPythonConsolePad consolePad;
		PythonTextEditorViewContent textEditorView;
		ITextEditor activeTextEditor;
		IPythonConsole pythonConsole;
		string lineFromActiveTextEditor;
		
		public SendLineToPythonConsoleCommand()
			: this(new PythonWorkbench())
		{
		}
		
		public SendLineToPythonConsoleCommand(IPythonWorkbench workbench)
		{
			this.workbench = workbench;
			
			textEditorView = new PythonTextEditorViewContent(workbench);
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
			consolePad = workbench.GetPythonConsolePad();
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
			pythonConsole = consolePad.PythonConsole;
		}
	}
}
