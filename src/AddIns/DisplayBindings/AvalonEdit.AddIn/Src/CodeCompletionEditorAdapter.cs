// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.AddIn.Snippets;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.AvalonEdit.AddIn
{
	class CodeCompletionEditorAdapter : AvalonEditTextEditorAdapter
	{
		SharpDevelopTextEditor textEditor;
		
		public CodeCompletionEditorAdapter(SharpDevelopTextEditor textEditor)
			: base(textEditor)
		{
			this.textEditor = textEditor;
		}
		
		public override ICompletionListWindow ShowCompletionWindow(ICompletionItemList data)
		{
			if (data == null || !data.Items.Any())
				return null;
			SharpDevelopCompletionWindow window = new SharpDevelopCompletionWindow(this, this.TextEditor.TextArea, data);
			textEditor.ShowCompletionWindow(window);
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
			textEditor.ShowInsightWindow(insightWindow);
			return insightWindow;
		}
		
		public override IInsightWindow ActiveInsightWindow {
			get { return textEditor.ActiveInsightWindow; }
		}
		
		public override ICompletionListWindow ActiveCompletionWindow {
			get { return textEditor.ActiveCompletionWindow; }
		}
		
		public override ITextEditorOptions Options {
			get { return CodeEditorOptions.Instance; }
		}
	}
}
