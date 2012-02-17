// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			
			if (char.IsLetterOrDigit (ch) || ch == '_') {
				//if (completionContext.TriggerOffset > 1 && char.IsLetterOrDigit (document.Editor.GetCharAt (completionContext.TriggerOffset - 2)))
				//	return null;
				list.PreselectionLength = 1;
			}
			list.Items.AddRange(cc.GetCompletionData(editor.Caret.Offset, false).Cast<ICompletionItem>());
			if (list.Items.Count > 0) {
				list.SortItems();
				list.SuggestedItem = list.Items.FirstOrDefault(i => i.Text == cc.DefaultCompletionString);
				editor.ShowCompletionWindow(list);
				return true;
			}
			return false;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			return false;
		}
	}
}

