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

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.Completion
{
	public class CSharpCompletionBinding : ICodeCompletionBinding
	{
		FileName contextFileName;
		TextLocation currentLocation;
		ITextSource fileContent;
		
		public CSharpCompletionBinding()
			: this(null, TextLocation.Empty, null)
		{
		}
		
		public CSharpCompletionBinding(FileName contextFileName, TextLocation currentLocation, ITextSource fileContent)
		{
			this.contextFileName = contextFileName;
			this.currentLocation = currentLocation;
			this.fileContent = fileContent;
		}
		
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
			CSharpCompletionContext completionContext;
			if (fileContent == null) {
				completionContext = CSharpCompletionContext.Get(editor);
			} else {
				completionContext = CSharpCompletionContext.Get(editor, fileContent, currentLocation, contextFileName);
			}
			if (completionContext == null)
				return false;
			
			int caretOffset;
			if (fileContent == null) {
				caretOffset = editor.Caret.Offset;
				currentLocation = editor.Caret.Location;
			} else {
				caretOffset = completionContext.Document.GetOffset(currentLocation);
			}
			
			var completionFactory = new CSharpCompletionDataFactory(completionContext, new CSharpResolver(completionContext.TypeResolveContextAtCaret));
			
			CSharpCompletionEngine cce = new CSharpCompletionEngine(
				completionContext.Document,
				completionContext.CompletionContextProvider,
				completionFactory,
				completionContext.ProjectContent,
				completionContext.TypeResolveContextAtCaret
			);
			var formattingOptions = CSharpFormattingPolicies.Instance.GetProjectOptions(completionContext.Compilation.GetProject());
			cce.FormattingPolicy = formattingOptions.OptionsContainer.GetEffectiveOptions();
			cce.EolMarker = DocumentUtilities.GetLineTerminator(completionContext.Document, currentLocation.Line);
			
			cce.IndentString = editor.Options.IndentationString;
			int startPos, triggerWordLength;
			IEnumerable<ICompletionData> completionData;
			if (ctrlSpace) {
				if (!cce.TryGetCompletionWord(caretOffset, out startPos, out triggerWordLength)) {
					startPos = caretOffset;
					triggerWordLength = 0;
				}
				completionData = cce.GetCompletionData(startPos, true);
				completionData = completionData.Concat(cce.GetImportCompletionData(startPos));
			} else {
				startPos = caretOffset;
				if (char.IsLetterOrDigit (completionChar) || completionChar == '_') {
					if (startPos > 1 && char.IsLetterOrDigit (completionContext.Document.GetCharAt (startPos - 2)))
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
			list.Items.AddRange(FilterAndAddTemplates(editor, completionData.Cast<ICompletionItem>().ToList()));
			if (list.Items.Count > 0) {
				list.SortItems();
				list.PreselectionLength = caretOffset - startPos;
				list.PostselectionLength = Math.Max(0, startPos + triggerWordLength - caretOffset);
				list.SuggestedItem = list.Items.FirstOrDefault(i => i.Text == cce.DefaultCompletionString);
				editor.ShowCompletionWindow(list);
				return true;
			}
			
			if (!ctrlSpace) {
				// Method Insight
				var pce = new CSharpParameterCompletionEngine(
					completionContext.Document,
					completionContext.CompletionContextProvider,
					completionFactory,
					completionContext.ProjectContent,
					completionContext.TypeResolveContextAtCaret
				);
				var newInsight = pce.GetParameterDataProvider(caretOffset, completionChar) as CSharpMethodInsight;
				if (newInsight != null && newInsight.items.Count > 0) {
					newInsight.UpdateHighlightedParameter(pce);
					newInsight.Show();
					return true;
				}
			}
			return false;
		}

		static IEnumerable<ICompletionItem> FilterAndAddTemplates(ITextEditor editor, IList<ICompletionItem> items)
		{
			List<ISnippetCompletionItem> snippets = editor.GetSnippets().ToList();
			snippets.RemoveAll(item => !FitsInContext(item, items));
			items.RemoveAll(item => ClassBrowserIconService.Keyword.Equals(item.Image) && snippets.Exists(i => i.Text == item.Text));
			items.AddRange(snippets);
			return items;
		}
		
		static bool FitsInContext(ISnippetCompletionItem item, IList<ICompletionItem> list)
		{
			if (string.IsNullOrEmpty(item.Keyword))
				return true;
			
			return list.Any(x => ClassBrowserIconService.Keyword.Equals(x.Image)
			                && x.Text == item.Keyword);
		}
	}
}
