// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 6077 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public static class CompletionDataHelper
	{
		public static ICompletionItemList GenerateCompletionData(this ExpressionResult expressionResult, ITextEditor editor, char pressedKey)
		{
			DefaultCompletionItemList result = new NRefactoryCompletionItemList();
			
			IResolver resolver = ParserService.CreateResolver(editor.FileName);
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			if (info == null)
				return result;
			
			List<ICompletionEntry> data = new List<ICompletionEntry>();
			
			if (expressionResult.Context != ExpressionContext.Global && expressionResult.Context != ExpressionContext.TypeDeclaration) {
				if (expressionResult.Context == ExpressionContext.Importable && expressionResult.Expression.Equals("Imports", StringComparison.OrdinalIgnoreCase)) {
					expressionResult.Expression = "Global";
				}
				var rr = resolver.Resolve(expressionResult, info, editor.Document.Text);
				
				if (rr == null) {
					if (IdentifierExpected(expressionResult.Tag))
						data = new NRefactoryResolver(LanguageProperties.VBNet)
							.CtrlSpace(editor.Caret.Line, editor.Caret.Column, info, editor.Document.Text, expressionResult.Context, ((NRefactoryCompletionItemList)result).ContainsItemsFromAllNamespaces);
				} else {
					data = rr.GetCompletionData(info.CompilationUnit.ProjectContent, ((NRefactoryCompletionItemList)result).ContainsItemsFromAllNamespaces) ?? data;
				}
			}
			
			bool addedKeywords = false;
			
			if (expressionResult.Tag != null && (expressionResult.Context != ExpressionContext.Importable) && pressedKey != '.') {
				AddVBNetKeywords(data, (BitArray)expressionResult.Tag);
				if (!((BitArray)expressionResult.Tag)[Tokens.New] &&  expressionResult.Context == ExpressionContext.Type)
					data.Add(new KeywordEntry("New"));
				addedKeywords = true;
			}
			
			result = CodeCompletionItemProvider.ConvertCompletionData(result, data, expressionResult.Context);
			
			if (addedKeywords)
				AddTemplates(editor, result);
			
			if (pressedKey == '\0') { // ctrl+space
				char prevChar =  editor.Caret.Offset > 0 ? editor.Document.GetCharAt(editor.Caret.Offset - 1) : '\0';
				string word = char.IsLetterOrDigit(prevChar) || prevChar == '_' ? editor.GetWordBeforeCaret() : "";
				
				if (!string.IsNullOrWhiteSpace(word))
					result.PreselectionLength = word.Length;
			}
			
			return result;
		}
		
		static bool IdentifierExpected(object tag)
		{
			if (tag is BitArray)
				return (tag as BitArray)[2];
			return false;
		}
		
		static void AddVBNetKeywords(List<ICompletionEntry> ar, BitArray keywords)
		{
			for (int i = 0; i < keywords.Length; i++) {
				if (keywords[i] && i >= Tokens.AddHandler && i < Tokens.MaxToken) {
					ar.Add(new KeywordEntry(Tokens.GetTokenString(i)));
				}
			}
		}
		
		static void AddTemplates(ITextEditor editor, DefaultCompletionItemList list)
		{
			if (list == null)
				return;
			List<ICompletionItem> snippets = editor.GetSnippets().ToList();
			list.Items.RemoveAll(item => item.Image == ClassBrowserIconService.Keyword && snippets.Exists(i => i.Text == item.Text));
			list.Items.AddRange(snippets);
			list.SortItems();
		}
	}
}
