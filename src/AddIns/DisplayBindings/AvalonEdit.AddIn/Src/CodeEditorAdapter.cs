// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps the CodeEditor class to provide the ITextEditor interface.
	/// </summary>
	public class CodeEditorAdapter : AvalonEditTextEditorAdapter
	{
		readonly CodeEditor codeEditor;
		
		public CodeEditorAdapter(CodeEditor codeEditor, TextEditor textEditor) : base(textEditor)
		{
			if (codeEditor == null)
				throw new ArgumentNullException("codeEditor");
			this.codeEditor = codeEditor;
		}
		
		public override string FileName {
			get { return codeEditor.FileName; }
		}
		
		public override IFormattingStrategy FormattingStrategy {
			get { return codeEditor.FormattingStrategy; }
		}
		
		public override void ShowCompletionWindow(ICompletionItemList data)
		{
			if (data == null || !data.Items.Any())
				return;
			CompletionWindow window = new SharpDevelopCompletionWindow(this, this.TextEditor.TextArea, data);
			codeEditor.NotifyCompletionWindowOpened(window);
			window.Show();
		}
		
		public override IInsightWindow ShowInsightWindow(IEnumerable<IInsightItem> items)
		{
			if (items == null)
				return null;
			var insightWindow = new SharpDevelopInsightWindow(this.TextEditor.TextArea);
			insightWindow.Items.AddRange(items);
			if (insightWindow.Items.Count > 0) {
				insightWindow.SelectedItem = insightWindow.Items[0];
			} else {
				// don't open insight window when there are no items
				return null;
			}
			codeEditor.NotifyInsightWindowOpened(insightWindow);
			insightWindow.Show();
			
			return insightWindow;
		}
		
		public override IInsightWindow ActiveInsightWindow {
			get { return codeEditor.ActiveInsightWindow; }
		}
	}
}
