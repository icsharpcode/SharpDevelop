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
	public class VBNetCodeCompletionDataProvider : NRefactoryCtrlSpaceCompletionItemProvider
	{
		ExpressionResult result;
		char pressed = '\0';
		
		public VBNetCodeCompletionDataProvider(ExpressionResult result, char ch)
			: base(LanguageProperties.VBNet, result.Context)
		{
			this.result = result;
			pressed = ch;
		}
		
		protected override List<ICompletionEntry> CtrlSpace(ITextEditor editor, ExpressionContext context)
		{
			var list = base.CtrlSpace(editor, context);
			
			BitArray expectedSet = result.Tag as BitArray;
			
			if (expectedSet != null)
				AddVBNetKeywords(list, expectedSet);
			
			// Inherits, Implements
			if (ExpressionContext.Type == context) {
				
			}
			
			if (ExpressionContext.MethodBody == context) {
				
			}
			
			return list;
		}
		
		public override ICompletionItemList GenerateCompletionList(ITextEditor editor)
		{
			if (result.Context == ExpressionContext.Importable && result.Expression == "Imports")
				return new CodeCompletionItemProvider().GenerateCompletionListForExpression(editor, new ExpressionResult("Global") { Context = ExpressionContext.Importable });
			
			if (pressed == '.')
				return new VBNetDotCodeCompletionItemProvider().GenerateCompletionList(editor);
			
			return base.GenerateCompletionList(editor);
		}
		
		class VBNetDotCodeCompletionItemProvider : DotCodeCompletionItemProvider
		{
			public override ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
			{
				// bypass ParserService.Resolve and set resolver.LimitMethodExtractionUntilCaretLine
				ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
				NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.VBNet);
				resolver.LimitMethodExtractionUntilLine = editor.Caret.Line;
				return resolver.Resolve(expressionResult, parseInfo, editor.Document.Text);
			}
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
