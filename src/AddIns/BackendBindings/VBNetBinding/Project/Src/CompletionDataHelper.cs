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
		public static ICompletionItemList GenerateCompletionData(this ExpressionResult expressionResult, ITextEditor editor)
		{
			var result = new NRefactoryCompletionItemList();
			
			IResolver resolver = ParserService.CreateResolver(editor.FileName);
			ParseInformation info = ParserService.GetParseInformation(editor.FileName);
			
			if (info == null)
				return result;
			
			List<ICompletionEntry> data = new List<ICompletionEntry>();
			
			if (string.IsNullOrEmpty(expressionResult.Expression)) {
				data = new NRefactoryResolver(LanguageProperties.VBNet)
					.CtrlSpace(editor.Caret.Line, editor.Caret.Column, info, editor.Document.Text, expressionResult.Context, result.ContainsItemsFromAllNamespaces);
			} else {
				if (expressionResult.Context != ExpressionContext.Global && expressionResult.Context != ExpressionContext.TypeDeclaration) {
					if (expressionResult.Context == ExpressionContext.Importable && expressionResult.Expression == "Imports") {
						expressionResult.Expression = "Global";
					}
					
					var rr = resolver.Resolve(expressionResult, info, editor.Document.Text);
					
					if (rr == null)
						return result;
					
					data = rr.GetCompletionData(info.CompilationUnit.ProjectContent, result.ContainsItemsFromAllNamespaces);
				}
			}
			
			if (expressionResult.Tag != null && (expressionResult.Context != ExpressionContext.Importable))
				AddVBNetKeywords(data, (BitArray)expressionResult.Tag);
			
			return CodeCompletionItemProvider.ConvertCompletionData(result, data, expressionResult.Context);
		}
		
		static void AddVBNetKeywords(List<ICompletionEntry> ar, BitArray keywords)
		{
			for (int i = 0; i < keywords.Length; i++) {
				if (keywords[i] && i >= Tokens.AddHandler && i < Tokens.MaxToken) {
					ar.Add(new KeywordEntry(Tokens.GetTokenString(i)));
				}
			}
		}
	}
}
