// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
