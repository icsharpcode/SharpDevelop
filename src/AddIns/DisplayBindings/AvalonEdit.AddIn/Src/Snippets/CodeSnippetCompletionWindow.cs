// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
