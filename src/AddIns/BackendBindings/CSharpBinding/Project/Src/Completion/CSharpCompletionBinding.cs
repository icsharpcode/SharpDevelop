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
using System.Linq;

using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	public class CSharpCompletionBinding : ICodeCompletionBinding
	{
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			// We use HandleKeyPressed instead.
			return CodeCompletionKeyPressResult.None;
		}
		
		public bool HandleKeyPressed(ITextEditor editor, char ch)
		{
			if (editor.ActiveCompletionWindow != null)
				return false;
			return ShowCompletion(editor, ch, false);
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return ShowCompletion(editor, '\0', true);
		}
		
		bool ShowCompletion(ITextEditor editor, char completionChar, bool ctrlSpace)
		{
			var completionContext = CSharpCompletionContext.Get(editor);
			if (completionContext == null)
				return false;
			
			var completionFactory = new CSharpCompletionDataFactory(completionContext, new CSharpResolver(completionContext.TypeResolveContextAtCaret));
			CSharpCompletionEngine cce = new CSharpCompletionEngine(
				editor.Document,
				completionContext.CompletionContextProvider,
				completionFactory,
				completionContext.ProjectContent,
				completionContext.TypeResolveContextAtCaret
			);
			
			cce.FormattingPolicy = FormattingOptionsFactory.CreateSharpDevelop();
			cce.EolMarker = DocumentUtilities.GetLineTerminator(editor.Document, editor.Caret.Line);
			cce.IndentString = editor.Options.IndentationString;
			
			int startPos, triggerWordLength;
			IEnumerable<ICompletionData> completionData;
			if (ctrlSpace) {
				if (!cce.TryGetCompletionWord(editor.Caret.Offset, out startPos, out triggerWordLength)) {
					startPos = editor.Caret.Offset;
					triggerWordLength = 0;
				}
				completionData = cce.GetCompletionData(startPos, true);
				completionData = completionData.Concat(cce.GetImportCompletionData(startPos));
			} else {
				startPos = editor.Caret.Offset;
				if (char.IsLetterOrDigit (completionChar) || completionChar == '_') {
					if (startPos > 1 && char.IsLetterOrDigit (editor.Document.GetCharAt (startPos - 2)))
						return false;
					completionData = cce.GetCompletionData(startPos, false);
					startPos--;
					triggerWordLength = 1;
				} else {
					completionData = cce.GetCompletionData(startPos, false);
					triggerWordLength = 0;
				}
			}
			
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			list.Items.AddRange(completionData.Cast<ICompletionItem>());
			if (list.Items.Count > 0) {
				list.SortItems();
				list.PreselectionLength = editor.Caret.Offset - startPos;
				list.PostselectionLength = Math.Max(0, startPos + triggerWordLength - editor.Caret.Offset);
				list.SuggestedItem = list.Items.FirstOrDefault(i => i.Text == cce.DefaultCompletionString);
				editor.ShowCompletionWindow(list);
				return true;
			}
			
			if (!ctrlSpace) {
				// Method Insight
				var pce = new CSharpParameterCompletionEngine(
					editor.Document,
					completionContext.CompletionContextProvider,
					completionFactory,
					completionContext.ProjectContent,
					completionContext.TypeResolveContextAtCaret
				);
				var newInsight = pce.GetParameterDataProvider(editor.Caret.Offset, completionChar) as CSharpMethodInsight;
				if (newInsight != null && newInsight.items.Count > 0) {
					newInsight.UpdateHighlightedParameter(pce);
					newInsight.Show();
					return true;
				}
			}
			return false;
		}
	}
}
