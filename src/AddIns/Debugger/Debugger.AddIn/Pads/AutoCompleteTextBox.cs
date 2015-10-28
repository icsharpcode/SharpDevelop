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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Pads.Controls
{
	public class AutoCompleteTextBox : UserControl
	{
		TextEditor editor;
		ITextEditor editorAdapter;
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteTextBox),
			                            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TextChanged));
		
		public static readonly DependencyProperty IsEditableProperty =
			DependencyProperty.Register("IsEditable", typeof(bool), typeof(AutoCompleteTextBox),
			                            new FrameworkPropertyMetadata(true, IsEditableChanged));
		
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		
		public bool IsEditable {
			get { return (bool)GetValue(IsEditableProperty); }
			set { SetValue(IsEditableProperty, value); }
		}

		public FileName ContextFileName { get; set; }
		public TextLocation ContextTextLocation { get; set; }
		
		static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AutoCompleteTextBox)d).editor.Text = (string)e.NewValue;
		}
		
		static void IsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((AutoCompleteTextBox)d).editor.IsReadOnly = !(bool)e.NewValue;
		}
		
		public AutoCompleteTextBox()
		{
			object tmp;
			this.editorAdapter = SD.EditorControlService.CreateEditor(out tmp);
			this.editor = (TextEditor)tmp;
			
			this.editor.Background = Brushes.Transparent;
			this.editor.ClearValue(TextEditor.FontFamilyProperty);
			this.editor.ClearValue(TextEditor.FontSizeProperty);
			this.editor.ShowLineNumbers = false;
			this.editor.Options.HighlightCurrentLine = false;
			this.editor.WordWrap = false;
			this.editor.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
			this.editor.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
			this.editor.TextArea.GotKeyboardFocus += delegate {
				this.Background = Brushes.White;
				this.Foreground = Brushes.Black;
			};
			this.editor.TextArea.LostKeyboardFocus += delegate {
				this.Background = Brushes.Transparent;
				this.ClearValue(ForegroundProperty);
				this.Text = this.editor.Text;
				this.editorAdapter.ClearSelection();
			};
			this.editor.TextArea.PreviewKeyDown += editor_TextArea_PreviewKeyDown;
			this.editor.TextArea.TextEntered += editor_TextArea_TextEntered;
			
			this.Content = this.editor.TextArea;
			this.messageView = new ToolTip { PlacementTarget = this, Placement = PlacementMode.Bottom, StaysOpen = true };
			
			HorizontalContentAlignment = HorizontalAlignment.Stretch;
			VerticalContentAlignment = VerticalAlignment.Stretch;
		}
		
		void editor_TextArea_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return || e.Key == Key.Escape) {
				if (e.Key == Key.Return) {
					this.Text = this.editor.Text;
					editor.SelectionStart = this.Text.Length;
				}
				
				e.Handled = true;
			}
		}
		
		ToolTip messageView;
		
		void editor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			if (e.Text == ".") {
				StackFrame frame = WindowsDebugger.CurrentStackFrame;
				if (frame != null) {
					ContextFileName = new FileName(frame.NextStatement.Filename);
					ContextTextLocation = new TextLocation(frame.NextStatement.StartLine, frame.NextStatement.StartColumn);
				}
				if (ContextFileName == null) return;
				var binding = DebuggerDotCompletion.PrepareDotCompletion(editor.Text.Substring(0, editor.CaretOffset), SD.ParserService.ResolveContext(ContextFileName, ContextTextLocation));
				if (binding == null) return;
				binding.HandleKeyPressed(editorAdapter, '.');
			} else {
				// TODO : implement automated error checking CSharpParser.ParseExpression does not report useful error messages.
//				Error[] errors;
//				if (!DebuggerDotCompletion.CheckSyntax(Text, out errors)) {
//					StringBuilder output = new StringBuilder();
//					foreach (var error in errors) {
//						output.AppendLine(error.Message + " at " + error.Region.Begin);
//					}
//					messageView.Content = output.ToString();
//					messageView.IsOpen = true;
//				} else {
//					messageView.IsOpen = false;
//				}
			}
		}
		
		public void FocusEditor()
		{
			editor.TextArea.Focus();
			editor.SelectAll();
		}
	}
}