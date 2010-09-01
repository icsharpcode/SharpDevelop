// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using AvalonEdit = ICSharpCode.AvalonEdit;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsolePad : AbstractPadContent, IRubyConsolePad
	{
		ThreadSafeRubyConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		RubyConsoleHost host;
		
		public RubyConsolePad()
		{
			textEditor = new AvalonEdit.TextEditor();
			consoleTextEditor = new ThreadSafeRubyConsoleTextEditor(textEditor);
			host = new RubyConsoleHost(consoleTextEditor);
			host.Run();
		}
		
		public IConsoleTextEditor ConsoleTextEditor {
			get { return consoleTextEditor; }
		}
		
		public IRubyConsole RubyConsole {
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
