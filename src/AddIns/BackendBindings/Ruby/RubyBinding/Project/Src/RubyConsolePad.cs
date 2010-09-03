// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop.Gui;
using AvalonEdit = ICSharpCode.AvalonEdit;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsolePad : AbstractPadContent, IScriptingConsolePad
	{
		ThreadSafeScriptingConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		RubyConsoleHost host;
		
		public RubyConsolePad()
		{
			textEditor = new AvalonEdit.TextEditor();
			consoleTextEditor = new ThreadSafeScriptingConsoleTextEditor(textEditor);
			host = new RubyConsoleHost(consoleTextEditor);
			host.Run();
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
