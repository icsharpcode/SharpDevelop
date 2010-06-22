// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.PythonBinding
{
	public delegate void ConsoleTextEditorKeyEventHandler(object source, ConsoleTextEditorKeyEventArgs e);
	
	public abstract class ConsoleTextEditorKeyEventArgs : EventArgs
	{
		Key key;
		
		public ConsoleTextEditorKeyEventArgs(Key key)
		{
			this.key = key;
		}
		
		public Key Key {
			get { return key; }
		}
		
		public abstract bool Handled {
			get; set;
		}
	}
}
