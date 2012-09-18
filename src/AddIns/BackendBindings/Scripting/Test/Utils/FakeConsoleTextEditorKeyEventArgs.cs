// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;
using ICSharpCode.Scripting;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FakeConsoleTextEditorKeyEventArgs : ConsoleTextEditorKeyEventArgs
	{
		bool handled;
		
		public FakeConsoleTextEditorKeyEventArgs(Key key, ModifierKeys modifiers = ModifierKeys.None)
			: base(key, modifiers)
		{
		}
		
		public override bool Handled {
			get { return handled; }
			set { handled = value; }
		}
	}
}
