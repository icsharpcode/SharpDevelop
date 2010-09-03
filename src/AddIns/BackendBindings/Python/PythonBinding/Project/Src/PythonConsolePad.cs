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

namespace ICSharpCode.PythonBinding
{
	public class PythonConsolePad : AbstractPadContent, IPythonConsolePad
	{
		ThreadSafePythonConsoleTextEditor consoleTextEditor;
		AvalonEdit.TextEditor textEditor;
		PythonConsoleHost host;
		
		public PythonConsolePad()
		{
			textEditor = new AvalonEdit.TextEditor();
			consoleTextEditor = new ThreadSafePythonConsoleTextEditor(textEditor);
			host = new PythonConsoleHost(consoleTextEditor);
			host.Run();	
		}
		
		public IScriptingConsoleTextEditor ConsoleTextEditor {
			get { return consoleTextEditor; }
		}
		
		public IPythonConsole PythonConsole {
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
