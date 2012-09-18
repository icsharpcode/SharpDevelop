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
		ModifierKeys modifiers;
		
		public ConsoleTextEditorKeyEventArgs(Key key, ModifierKeys modifiers)
		{
			this.key = key;
			this.modifiers = modifiers;
		}
		
		public Key Key {
			get { return key; }
		}
		
		public ModifierKeys Modifiers {
			get { return modifiers; }
		}
		
		public abstract bool Handled {
			get; set;
		}
	}
}
