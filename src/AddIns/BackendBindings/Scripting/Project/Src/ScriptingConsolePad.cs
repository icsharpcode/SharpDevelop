// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Scripting
{
	public abstract class ScriptingConsolePad : AbstractPadContent, IScriptingConsolePad
	{
		ScriptingConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		IScriptingConsoleHost host;
		
		public ScriptingConsolePad()
		{
			textEditor = TextEditorFactory.CreateTextEditor();
			textEditor.SyntaxHighlighting = GetSyntaxHighlighting();
			CreateConsoleHost();
			host.Run();	
		}
		
		IHighlightingDefinition GetSyntaxHighlighting()
		{
			return HighlightingManager.Instance.GetDefinition(SyntaxHighlightingName);
		}
		
		protected abstract string SyntaxHighlightingName { get; }
		
		void CreateConsoleHost()
		{
			ControlDispatcher dispatcher = new ControlDispatcher(textEditor);
			consoleTextEditor = new ScriptingConsoleTextEditor(textEditor);
			host = CreateConsoleHost(consoleTextEditor, dispatcher);
		}
		
		protected abstract IScriptingConsoleHost CreateConsoleHost(
			IScriptingConsoleTextEditor consoleTextEditor,
			IControlDispatcher dispatcher);
		
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
