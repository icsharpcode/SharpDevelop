// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using VBTokens = ICSharpCode.NRefactory.Parser.VB.Tokens;

namespace VBNetBinding
{
	public class VBNetCompletionBinding : NRefactoryCodeCompletionBinding
	{
		public VBNetCompletionBinding()
			: base(SupportedLanguage.VBNet)
		{
			// Don't use indexer insight for '[', VB uses '(' for indexer access
			this.EnableIndexerInsight = false;
		}
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			if(ch == '(' && EnableMethodInsight && CodeCompletionOptions.InsightEnabled) {
				editor.ShowInsightWindow(new MethodInsightDataProvider());
				return true;
			} else if(ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled) {
				if (InsightRefreshOnComma(editor, ch))
					return true;
			} else if (ch == '\n') {
				TryDeclarationTypeInference(editor, editor.Document.GetLineSegmentForOffset(editor.ActiveTextAreaControl.Caret.Offset));
			}
			return base.HandleKeyPress(editor, ch);
		}
		
		public override bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			// TODO: Assistance writing Methods/Fields/Properties/Events:
			// use public/static/etc. as keywords to display a list with other modifiers
			// and possible return types.
			switch (word.ToLowerInvariant()) {
				case "imports":
					editor.ShowCompletionWindow(new CodeCompletionDataProvider(new ExpressionResult("Global", ExpressionContext.Importable)), ' ');
					return true;
				case "as":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
					return true;
				case "new":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.ObjectCreation), ' ');
					return true;
				case "inherits":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
					return true;
				case "implements":
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Interface), ' ');
					return true;
				case "overrides":
					editor.ShowCompletionWindow(new OverrideCompletionDataProvider(), ' ');
					return true;
				case "return":
					IMember m = GetCurrentMember(editor);
					if (m != null) {
						ProvideContextCompletion(editor, m.ReturnType, ' ');
						return true;
					} else {
						goto default;
					}
				case "option":
					editor.ShowCompletionWindow(new TextCompletionDataProvider("Explicit On",
					                                                           "Explicit Off",
					                                                           "Strict On",
					                                                           "Strict Off",
					                                                           "Compare Binary",
					                                                           "Compare Text"), ' ');
					return true;
				default:
					return base.HandleKeyword(editor, word);
			}
		}
		
		bool TryDeclarationTypeInference(SharpDevelopTextAreaControl editor, LineSegment curLine)
		{
			string lineText = editor.Document.GetText(curLine.Offset, curLine.Length);
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new System.IO.StringReader(lineText));
			if (lexer.NextToken().kind != VBTokens.Dim)
				return false;
			if (lexer.NextToken().kind != VBTokens.Identifier)
				return false;
			if (lexer.NextToken().kind != VBTokens.As)
				return false;
			Token t1 = lexer.NextToken();
			if (t1.kind != VBTokens.QuestionMark)
				return false;
			Token t2 = lexer.NextToken();
			if (t2.kind != VBTokens.Assign)
				return false;
			string expr = lineText.Substring(t2.col);
			LoggingService.Debug("DeclarationTypeInference: >" + expr + "<");
			ResolveResult rr = ParserService.Resolve(new ExpressionResult(expr),
			                                         editor.ActiveTextAreaControl.Caret.Line + 1,
			                                         t2.col, editor.FileName,
			                                         editor.Document.TextContent);
			if (rr != null && rr.ResolvedType != null) {
				ClassFinder context = new ClassFinder(editor.FileName, editor.ActiveTextAreaControl.Caret.Line, t1.col);
				if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
					VBNetAmbience.Instance.ConversionFlags = ConversionFlags.None;
				else
					VBNetAmbience.Instance.ConversionFlags = ConversionFlags.UseFullyQualifiedNames;
				string typeName = VBNetAmbience.Instance.Convert(rr.ResolvedType);
				editor.Document.Replace(curLine.Offset + t1.col - 1, 1, typeName);
				editor.ActiveTextAreaControl.Caret.Column += typeName.Length - 1;
				return true;
			}
			return false;
		}
	}
}
