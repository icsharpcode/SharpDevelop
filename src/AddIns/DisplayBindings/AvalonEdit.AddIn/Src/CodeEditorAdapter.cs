// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Indentation;
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Wraps the CodeEditor class to provide the ITextEditor interface.
	/// </summary>
	sealed class CodeEditorAdapter : AvalonEditTextEditorAdapter
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
		
		public override ICompletionListWindow ActiveCompletionWindow {
			get { return codeEditor.ActiveCompletionWindow; }
		}
		
		ILanguageBinding languageBinding;
		
		public override ILanguageBinding Language {
			get { return languageBinding; }
		}
		
		internal void FileNameChanged()
		{
			if (languageBinding != null)
				languageBinding.Detach();
			
			languageBinding = LanguageBindingService.CreateBinding(this);
			if (languageBinding != null) {
				languageBinding.Attach(this);
				
				// update properties set by languageBinding
				this.TextEditor.TextArea.IndentationStrategy = new IndentationStrategyAdapter(this, languageBinding.FormattingStrategy);
			} else {
				this.TextEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			}
		}
		
		public override ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			if (data == null || !data.Items.Any())
				return null;
			SharpDevelopCompletionWindow window = new SharpDevelopCompletionWindow(this, this.TextEditor.TextArea, data);
			codeEditor.ShowCompletionWindow(window);
			return window;
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
			codeEditor.ShowInsightWindow(insightWindow);
			return insightWindow;
		}
		
		public override IInsightWindow ActiveInsightWindow {
			get { return codeEditor.ActiveInsightWindow; }
		}
		
		public override ITextEditor PrimaryView {
			get { return codeEditor.PrimaryTextEditorAdapter; }
		}
	}
}
