// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.Scripting
{
	public class SendLineToScriptingConsoleCommand : SendToScriptingConsoleCommand
	{
		string lineFromActiveTextEditor;
		
		public SendLineToScriptingConsoleCommand(IScriptingWorkbench workbench)
			: base(workbench)
		{
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
			IDocumentLine documentLine =  activeTextEditor.Document.GetLineByNumber(lineNumber);
			lineFromActiveTextEditor = activeTextEditor.Document.GetText(documentLine);
		}
		
		void AppendLineToScriptingConsole()
		{
			GetScriptingConsole();
			scriptingConsole.SendLine(lineFromActiveTextEditor);
		}
	}
}
