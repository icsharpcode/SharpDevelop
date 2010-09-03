// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
