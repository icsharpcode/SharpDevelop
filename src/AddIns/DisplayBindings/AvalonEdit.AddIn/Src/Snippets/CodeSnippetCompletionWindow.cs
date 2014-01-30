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
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// Completion window used for code snippets.
	/// </summary>
	public class CodeSnippetCompletionWindow : SharpDevelopCompletionWindow
	{
		TextBox snippetInput;
		
		public CodeSnippetCompletionWindow(ITextEditor editor, ICompletionItemList list)
			: base(editor, editor.GetService(typeof(TextArea)) as TextArea, list)
		{
			this.snippetInput = new TextBox();
			
			DockPanel panel = new DockPanel() {
				Children = {
					snippetInput
				}
			};
			
			this.Content = panel;
			
			panel.Children.Add(CompletionList);
			
			snippetInput.SetValue(DockPanel.DockProperty, Dock.Top);
			
			this.Width = 150;
			this.Height = 200;
			
			this.Loaded += delegate { Keyboard.Focus(snippetInput); };
			this.snippetInput.PreviewKeyDown += new KeyEventHandler(CodeSnippetCompletionWindowPreviewKeyDown);
			this.snippetInput.TextChanged += new TextChangedEventHandler(CodeSnippetCompletionWindow_TextChanged);
		}
		
		void CodeSnippetCompletionWindowPreviewKeyDown(object sender, KeyEventArgs e)
		{
			CompletionList.HandleKey(e);
		}
		
		void CodeSnippetCompletionWindow_TextChanged(object sender, TextChangedEventArgs e)
		{
			CompletionList.SelectItem(this.snippetInput.Text);
		}
		
		protected override void ActivateParentWindow()
		{
		}
	}
}
