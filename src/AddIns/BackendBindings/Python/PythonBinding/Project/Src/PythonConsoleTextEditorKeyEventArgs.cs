// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Input;
using ICSharpCode.Scripting;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsoleTextEditorKeyEventArgs : ConsoleTextEditorKeyEventArgs
	{
		KeyEventArgs e;
		
		public PythonConsoleTextEditorKeyEventArgs(KeyEventArgs e)
			: base(e.Key)
		{
			this.e = e;
		}
		
		public override bool Handled {
			get { return e.Handled; }
			set { e.Handled = value; }
		}
	}
}
