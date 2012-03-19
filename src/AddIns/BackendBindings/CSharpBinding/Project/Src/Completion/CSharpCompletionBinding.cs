// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using CSharpBinding.Parser;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Parser;

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
			// Don't require the very latest parse information, an older cached version is OK.
			var parseInfo = ParserService.GetCachedParseInformation(editor.FileName) as CSharpFullParseInformation;
			if (parseInfo == null) {
				parseInfo = ParserService.Parse(editor.FileName, editor.Document) as CSharpFullParseInformation;
				if (parseInfo == null)
					return false;
			}
			ICompilation compilation = ParserService.GetCompilationForFile(editor.FileName);
			var pc = compilation.MainAssembly.UnresolvedAssembly as IProjectContent;
			if (pc == null)
				return false;
			
			CSharpCompletionEngine cc = new CSharpCompletionEngine(
				editor.Document,
				new CSharpCompletionDataFactory(),
				pc,
				parseInfo.ParsedFile.GetTypeResolveContext(compilation, editor.Caret.Location),
				parseInfo.CompilationUnit,
				parseInfo.ParsedFile
			);
			
			//cc.FormattingPolicy = ?
			cc.EolMarker = DocumentUtilitites.GetLineTerminator(editor.Document, editor.Caret.Line);
			//cc.IndentString = ?
			
			int startPos, triggerWordLength;
			IEnumerable<ICompletionData> completionData;
			if (ctrlSpace) {
				if (!cc.TryGetCompletionWord(editor.Caret.Offset, out startPos, out triggerWordLength)) {
					startPos = editor.Caret.Offset;
					triggerWordLength = 0;
				}
				completionData = cc.GetCompletionData(startPos, true);
			} else {
				startPos = editor.Caret.Offset;
				if (char.IsLetterOrDigit (completionChar) || completionChar == '_') {
					if (startPos > 1 && char.IsLetterOrDigit (editor.Document.GetCharAt (startPos - 2)))
						return false;
					completionData = cc.GetCompletionData(startPos, false);
					startPos--;
					triggerWordLength = 1;
				} else {
					completionData = cc.GetCompletionData(startPos, false);
					triggerWordLength = 0;
				}
			}
			
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			list.Items.AddRange(completionData.Cast<ICompletionItem>());
			if (list.Items.Count > 0) {
				list.SortItems();
				list.PreselectionLength = editor.Caret.Offset - startPos;
				list.PostselectionLength = Math.Max(0, startPos + triggerWordLength - editor.Caret.Offset);
				list.SuggestedItem = list.Items.FirstOrDefault(i => i.Text == cc.DefaultCompletionString);
				editor.ShowCompletionWindow(list);
				return true;
			}
			return false;
		}
	}
}

