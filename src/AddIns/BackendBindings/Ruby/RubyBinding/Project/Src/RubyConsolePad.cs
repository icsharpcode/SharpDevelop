// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsolePad : AbstractPadContent, IScriptingConsolePad
	{
		ScriptingConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		RubyConsoleHost host;
		
		public RubyConsolePad()
		{
			textEditor = CreateTextEditor();
			ControlDispatcher dispatcher = new ControlDispatcher(textEditor);
			consoleTextEditor = new ScriptingConsoleTextEditor(textEditor);
			host = new RubyConsoleHost(consoleTextEditor, dispatcher);
			host.Run();
		}
		
		AvalonEdit.TextEditor CreateTextEditor()
		{
			object textEditor;
			EditorControlService.CreateEditor(out textEditor);
			return (AvalonEdit.TextEditor)textEditor;
		}
		
		public IScriptingConsoleTextEditor ScriptingConsoleTextEditor {
			get { return consoleTextEditor; }
		}
		
		public IScriptingConsole ScriptingConsole {
			get { return host.RubyConsole; }
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
