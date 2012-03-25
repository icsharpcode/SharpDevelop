// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Linq;
using Debugger;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class RunToCursorCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			
			if (provider == null || WindowsDebugger.CurrentProcess == null)
				return;
			
			ITextEditor textEditor = provider.TextEditor;
			WindowsDebugger.CurrentProcess.RunTo(textEditor.FileName, textEditor.Caret.Line, textEditor.Caret.Column);
		}
	}
}
