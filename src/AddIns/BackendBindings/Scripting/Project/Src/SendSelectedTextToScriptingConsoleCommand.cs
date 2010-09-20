// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting
{
	public class SendSelectedTextToScriptingConsoleCommand : SendToScriptingConsoleCommand
	{
		string selectedText;
		
		public SendSelectedTextToScriptingConsoleCommand(IScriptingWorkbench workbench)
			: base(workbench)
		{
		}
		
		public override void Run()
		{
			GetSelectedTextInActiveTextEditor();
			GetScriptingConsolePad();
			ShowScriptingConsolePad();
			SendTextToScriptingConsole();
		}
		
		void GetSelectedTextInActiveTextEditor()
		{
			selectedText = activeTextEditor.SelectedText;
		}
		
		void SendTextToScriptingConsole()
		{
			GetScriptingConsole();
			scriptingConsole.SendText(selectedText);
		}
	}
}
