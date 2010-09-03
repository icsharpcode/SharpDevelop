// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
