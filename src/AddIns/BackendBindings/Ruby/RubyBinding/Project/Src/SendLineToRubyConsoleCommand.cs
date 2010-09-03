// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	public class SendLineToRubyConsoleCommand : AbstractCommand
	{
		IRubyWorkbench workbench;
		IRubyConsolePad consolePad;
		RubyTextEditorViewContent textEditorView;
		ITextEditor activeTextEditor;
		IRubyConsole RubyConsole;
		string lineFromActiveTextEditor;
		
		public SendLineToRubyConsoleCommand()
			: this(new RubyWorkbench())
		{
		}
		
		public SendLineToRubyConsoleCommand(IRubyWorkbench workbench)
		{
			this.workbench = workbench;
			
			textEditorView = new RubyTextEditorViewContent(workbench);
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
			consolePad = workbench.GetRubyConsolePad();
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
			RubyConsole = consolePad.RubyConsole;
		}
	}
}
