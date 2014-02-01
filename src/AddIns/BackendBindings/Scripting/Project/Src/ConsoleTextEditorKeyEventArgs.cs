// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
