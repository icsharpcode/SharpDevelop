// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

namespace ICSharpCode.Scripting
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
