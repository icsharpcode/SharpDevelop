// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
	/// Description of CodeSnippetCompletionWindow.
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
		}

		void CodeSnippetCompletionWindowPreviewKeyDown(object sender, KeyEventArgs e)
		{
			CompletionList.HandleKey(e);
			if (!e.Handled)
				CompletionList.SelectItemWithStart(this.snippetInput.Text);
		}
		
		protected override void ActiveParentWindow()
		{
		}
	}
}
