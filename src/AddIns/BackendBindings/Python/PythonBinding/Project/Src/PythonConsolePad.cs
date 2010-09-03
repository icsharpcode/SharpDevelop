// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Gui;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsolePad : AbstractPadContent, IScriptingConsolePad
	{
		ThreadSafeScriptingConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		PythonConsoleHost host;
		
		public PythonConsolePad()
		{
			textEditor = new AvalonEdit.TextEditor();
			consoleTextEditor = new ThreadSafeScriptingConsoleTextEditor(textEditor);
			host = new PythonConsoleHost(consoleTextEditor);
			host.Run();	
		}
		
		public IScriptingConsoleTextEditor ScriptingConsoleTextEditor {
			get { return consoleTextEditor; }
		}
		
		public IScriptingConsole ScriptingConsole {
			get { return host.PythonConsole; }
		}
		
		public override object Control {
			get { return textEditor; }
		}
		
		public override void Dispose()
		{
			host.Dispose();
		}
		
		public override object InitiallyFocusedControl {
			get { return textEditor; }
		}
	}
}
