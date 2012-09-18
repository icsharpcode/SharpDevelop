// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.Scripting
{
	public class ScriptingConsoleTextEditorKeyEventArgs : ConsoleTextEditorKeyEventArgs
	{
		KeyEventArgs e;
		
		public ScriptingConsoleTextEditorKeyEventArgs(KeyEventArgs e)
			: base(e.Key, e.KeyboardDevice.Modifiers)
		{
			this.e = e;
		}
		
		public override bool Handled {
			get { return e.Handled; }
			set { e.Handled = value; }
		}
	}
}
