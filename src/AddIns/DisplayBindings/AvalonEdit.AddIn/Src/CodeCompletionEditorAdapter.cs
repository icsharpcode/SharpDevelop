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
			var insightWindow = textEditor.ActiveInsightWindow;
			bool isNewWindow = false;
			if (insightWindow == null) {
				insightWindow = new SharpDevelopInsightWindow(this.TextEditor.TextArea);
				isNewWindow = true;
			}
			var adapter = new SharpDevelopInsightWindowAdapter(insightWindow);
			adapter.Items.AddRange(items);
			if (adapter.Items.Count > 0) {
				adapter.SelectedItem = adapter.Items[0];
			} else {
				// don't open insight window when there are no items
				return null;
			}
			insightWindow.SetActiveAdapter(adapter, isNewWindow);
			if (isNewWindow)
			{
				textEditor.ShowInsightWindow(insightWindow);
			}
			return adapter;
		}
		
		public override IInsightWindow ActiveInsightWindow {
			get {
				if (textEditor.ActiveInsightWindow != null)
					return textEditor.ActiveInsightWindow.activeAdapter;
				else
					return null;
			}
		}
		
		public override ICompletionListWindow ActiveCompletionWindow {
			get { return textEditor.ActiveCompletionWindow; }
		}
		
		public override ITextEditorOptions Options {
			get { return CodeEditorOptions.Instance; }
		}
	}
}
