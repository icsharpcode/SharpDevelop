// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
