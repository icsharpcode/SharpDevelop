// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace ICSharpCode.Scripting
{
	public abstract class ScriptingConsolePad : AbstractPadContent, IScriptingConsolePad
	{
		ScriptingConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		IScriptingConsoleHost host;
		
		public ScriptingConsolePad()
		{
			textEditor = CreateTextEditor();
			CreateConsoleHost();
			host.Run();	
		}
		
		AvalonEdit.TextEditor CreateTextEditor()
		{
			object textEditor;
			EditorControlService.CreateEditor(out textEditor);
			return (AvalonEdit.TextEditor)textEditor;
		}
		
		void CreateConsoleHost()
		{
			ControlDispatcher dispatcher = new ControlDispatcher(textEditor);
			consoleTextEditor = new ScriptingConsoleTextEditor(textEditor);
			host = CreateConsoleHost(consoleTextEditor, dispatcher);
		}
		
		protected virtual IScriptingConsoleHost CreateConsoleHost(
			IScriptingConsoleTextEditor consoleTextEditor,
			IControlDispatcher dispatcher)
		{
			return null;
		}
		
		public IScriptingConsoleTextEditor ScriptingConsoleTextEditor {
			get { return consoleTextEditor; }
		}
		
		public IScriptingConsole ScriptingConsole {
			get { return host.ScriptingConsole; }
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
