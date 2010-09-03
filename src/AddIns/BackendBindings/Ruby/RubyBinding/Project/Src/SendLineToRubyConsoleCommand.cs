// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	public class SendLineToRubyConsoleCommand : AbstractCommand
	{
		IScriptingWorkbench workbench;
		IScriptingConsolePad consolePad;
		ScriptingTextEditorViewContent textEditorView;
		ITextEditor activeTextEditor;
		IScriptingConsole RubyConsole;
		string lineFromActiveTextEditor;
		
		public SendLineToRubyConsoleCommand()
			: this(new RubyWorkbench())
		{
		}
		
		public SendLineToRubyConsoleCommand(IScriptingWorkbench workbench)
		{
			this.workbench = workbench;
			
			textEditorView = new ScriptingTextEditorViewContent(workbench);
			activeTextEditor = textEditorView.TextEditor;
		}	
		
		public override void Run()
		{
			GetLineFromActiveTextEditor();
			GetRubyConsolePad();
			ShowRubyConsolePad();
			AppendLineToRubyConsole();
		}
		
		void GetLineFromActiveTextEditor()
		{
			int lineNumber = activeTextEditor.Caret.Line;
			IDocumentLine documentLine =  activeTextEditor.Document.GetLine(lineNumber);
			lineFromActiveTextEditor = documentLine.Text;
		}
		
		void GetRubyConsolePad()
		{
			consolePad = workbench.GetScriptingConsolePad();
		}
				
		void ShowRubyConsolePad()
		{
			consolePad.BringToFront();
		}
		
		void AppendLineToRubyConsole()
		{
			GetRubyConsole();
			RubyConsole.SendLine(lineFromActiveTextEditor);
		}
		
		void GetRubyConsole()
		{
			RubyConsole = consolePad.ScriptingConsole;
		}
	}
}
