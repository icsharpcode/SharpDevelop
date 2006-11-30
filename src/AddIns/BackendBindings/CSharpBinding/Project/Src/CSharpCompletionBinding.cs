// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using AST = ICSharpCode.NRefactory.Ast;
using CSTokens = ICSharpCode.NRefactory.Parser.CSharp.Tokens;

namespace CSharpBinding
{
	public class CSharpCompletionBinding : NRefactoryCodeCompletionBinding
	{
		public CSharpCompletionBinding() : base(SupportedLanguage.CSharp)
		{
		}
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
			ExpressionContext context = null;
			if (ch == '(') {
				if (CodeCompletionOptions.KeywordCompletionEnabled) {
					switch (editor.GetWordBeforeCaret().Trim()) {
						case "for":
						case "lock":
							context = ExpressionContext.Default;
							break;
						case "using":
							context = ExpressionContext.TypeDerivingFrom(ParserService.CurrentProjectContent.GetClass("System.IDisposable"), false);
							break;
						case "catch":
							context = ExpressionContext.TypeDerivingFrom(ParserService.CurrentProjectContent.GetClass("System.Exception"), false);
							break;
						case "foreach":
						case "typeof":
						case "sizeof":
						case "default":
							context = ExpressionContext.Type;
							break;
					}
				}
				if (context != null) {
					if (IsInComment(editor)) return false;
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ch);
					return true;
				} else if (EnableMethodInsight && CodeCompletionOptions.InsightEnabled) {
					editor.ShowInsightWindow(new MethodInsightDataProvider());
					return true;
				}
				return false;
			} else if (ch == '[') {
				LineSegment line = editor.Document.GetLineSegmentForOffset(cursor);
				if (TextUtilities.FindPrevWordStart(editor.Document, cursor) <= line.Offset) {
					// [ is first character on the line
					// -> Attribute completion
					editor.ShowCompletionWindow(new AttributesDataProvider(ParserService.CurrentProjectContent), ch);
					return true;
				}
			} else if (ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled) {
				if (InsightRefreshOnComma(editor, ch))
					return true;
			} else if(ch == '=') {
				LineSegment curLine = editor.Document.GetLineSegmentForOffset(cursor);
				string documentText = editor.Text;
				int position = editor.ActiveTextAreaControl.Caret.Offset - 2;
				
				if (position > 0 && (documentText[position + 1] == '+')) {
					ExpressionResult result = ef.FindFullExpression(documentText, position);
					
					if(result.Expression != null) {
						ResolveResult resolveResult = ParserService.Resolve(result, editor.ActiveTextAreaControl.Caret.Line + 1, editor.ActiveTextAreaControl.Caret.Column + 1, editor.FileName, documentText);
						if (resolveResult != null && resolveResult.ResolvedType != null) {
							IClass underlyingClass = resolveResult.ResolvedType.GetUnderlyingClass();
							if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ParserService.CurrentProjectContent.GetClass("System.MulticastDelegate"))) {
								EventHandlerCompletitionDataProvider eventHandlerProvider = new EventHandlerCompletitionDataProvider(result.Expression, resolveResult);
								eventHandlerProvider.InsertSpace = true;
								editor.ShowCompletionWindow(eventHandlerProvider, ch);
							}
						}
					}
				} else if (position > 0) {
					ExpressionResult result = ef.FindFullExpression(documentText, position);
					
					if(result.Expression != null) {
						ResolveResult resolveResult = ParserService.Resolve(result, editor.ActiveTextAreaControl.Caret.Line + 1, editor.ActiveTextAreaControl.Caret.Column + 1, editor.FileName, documentText);
						if (resolveResult != null && resolveResult.ResolvedType != null) {
							if (ProvideContextCompletion(editor, resolveResult.ResolvedType, ch)) {
								return true;
							}
						}
					}
				}
			} else if (ch == ';') {
				LineSegment curLine = editor.Document.GetLineSegmentForOffset(cursor);
				// don't return true when inference succeeds, otherwise the ';' won't be added to the document.
				TryDeclarationTypeInference(editor, curLine);
			}
			
			return base.HandleKeyPress(editor, ch);
		}
		
		bool TryDeclarationTypeInference(SharpDevelopTextAreaControl editor, LineSegment curLine)
		{
			string lineText = editor.Document.GetText(curLine.Offset, curLine.Length);
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.CSharp, new System.IO.StringReader(lineText));
			Token typeToken = lexer.NextToken();
			if (typeToken.kind == CSTokens.Question) {
				if (lexer.NextToken().kind == CSTokens.Identifier) {
					Token t = lexer.NextToken();
					if (t.kind == CSTokens.Assign) {
						string expr = lineText.Substring(t.col);
						LoggingService.Debug("DeclarationTypeInference: >" + expr + "<");
						ResolveResult rr = ParserService.Resolve(new ExpressionResult(expr),
						                                         editor.ActiveTextAreaControl.Caret.Line + 1,
						                                         t.col, editor.FileName,
						                                         editor.Document.TextContent);
						if (rr != null && rr.ResolvedType != null) {
							ClassFinder context = new ClassFinder(editor.FileName, editor.ActiveTextAreaControl.Caret.Line, t.col);
							if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
								CSharpAmbience.Instance.ConversionFlags = ConversionFlags.None;
							else
								CSharpAmbience.Instance.ConversionFlags = ConversionFlags.UseFullyQualifiedNames;
							string typeName = CSharpAmbience.Instance.Convert(rr.ResolvedType);
							editor.Document.Replace(curLine.Offset + typeToken.col - 1, 1, typeName);
							editor.ActiveTextAreaControl.Caret.Column += typeName.Length - 1;
							return true;
						}
					}
				}
			}
			return false;
		}
		
		bool IsInComment(SharpDevelopTextAreaControl editor)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset - 1;
			return ef.FilterComments(editor.Document.GetText(0, cursor + 1), ref cursor) == null;
		}
		
		public override bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			// TODO: Assistance writing Methods/Fields/Properties/Events:
			// use public/static/etc. as keywords to display a list with other modifiers
			// and possible return types.
			switch (word) {
				case "using":
					if (IsInComment(editor)) return false;
					// TODO: check if we are inside class/namespace
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Namespace), ' ');
					return true;
				case "as":
				case "is":
					if (IsInComment(editor)) return false;
					editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Type), ' ');
					return true;
				case "override":
					if (IsInComment(editor)) return false;
					editor.ShowCompletionWindow(new OverrideCompletionDataProvider(), ' ');
					return true;
				case "new":
					return ShowNewCompletion(editor);
				case "case":
					return DoCaseCompletion(editor);
				case "return":
					IMember m = GetCurrentMember(editor);
					if (m != null) {
						return ProvideContextCompletion(editor, m.ReturnType, ' ');
					} else {
						goto default;
					}
				default:
					return base.HandleKeyword(editor, word);
			}
		}
		
		bool ShowNewCompletion(SharpDevelopTextAreaControl editor)
		{
			CSharpExpressionFinder ef = new CSharpExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
			ExpressionContext context = ef.FindExpression(editor.Document.GetText(0, cursor) + " T.", cursor + 2).Context;
			if (context.IsObjectCreation) {
				editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ' ');
				return true;
			}
			return false;
		}
		
		#region "case"-keyword completion
		bool DoCaseCompletion(SharpDevelopTextAreaControl editor)
		{
			ICSharpCode.TextEditor.Caret caret = editor.ActiveTextAreaControl.Caret;
			NRefactoryResolver r = new NRefactoryResolver(ParserService.CurrentProjectContent, LanguageProperties.CSharp);
			if (r.Initialize(editor.FileName, caret.Line + 1, caret.Column + 1)) {
				AST.INode currentMember = r.ParseCurrentMember(editor.Text);
				if (currentMember != null) {
					CaseCompletionSwitchFinder ccsf = new CaseCompletionSwitchFinder(caret.Line + 1, caret.Column + 1);
					currentMember.AcceptVisitor(ccsf, null);
					if (ccsf.bestStatement != null) {
						r.RunLookupTableVisitor(currentMember);
						ResolveResult rr = r.ResolveInternal(ccsf.bestStatement.SwitchExpression, ExpressionContext.Default);
						if (rr != null && rr.ResolvedType != null) {
							return ProvideContextCompletion(editor, rr.ResolvedType, ' ');
						}
					}
				}
			}
			return false;
		}
		
		private class CaseCompletionSwitchFinder : AbstractAstVisitor
		{
			Location caretLocation;
			internal AST.SwitchStatement bestStatement;
			
			public CaseCompletionSwitchFinder(int caretLine, int caretColumn)
			{
				caretLocation = new Location(caretColumn, caretLine);
			}
			
			public override object VisitSwitchStatement(AST.SwitchStatement switchStatement, object data)
			{
				if (switchStatement.StartLocation < caretLocation && caretLocation < switchStatement.EndLocation) {
					bestStatement = switchStatement;
				}
				return base.VisitSwitchStatement(switchStatement, data);
			}
		}
		#endregion
	}
}
