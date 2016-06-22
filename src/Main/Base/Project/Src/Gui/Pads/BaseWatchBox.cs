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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// Interaction logic for AbstractWatchBox.xaml
	/// </summary>
	public class BaseWatchBox : Window
	{		
		protected ConsoleControl console;	
		
		public BaseWatchBox()
		{
			console = new ConsoleControl();
									
			this.console.editor.TextArea.TextEntered += new TextCompositionEventHandler(AbstractConsolePadTextEntered);
			
			this.console.editor.TextArea.PreviewKeyDown += (sender, e) => {
				e.Handled = e.Key == Key.Return;
				
				if (e.Handled) {
					DialogResult = true;
					this.Close();
				}
			};
			
			// hide scroll bar
			this.console.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
			this.console.editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
			this.Loaded += delegate { this.console.editor.TextArea.Focus(); };
		}
		
		protected virtual void AbstractConsolePadTextEntered(object sender, TextCompositionEventArgs e)
		{
		}
		
		protected ITextEditor TextEditor {
			get {
				return console.TextEditor;
			}
		}
		
		/// <summary>
		/// Gets/sets the command text displayed at the command prompt.
		/// </summary>
		public string CommandText { 
			get { return TextEditor.Document.Text; }
		}
	}
}
