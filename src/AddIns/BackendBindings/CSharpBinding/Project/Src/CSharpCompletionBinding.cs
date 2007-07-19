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
		
		static CSharpExpressionFinder CreateExpressionFinder(string fileName)
		{
			return new CSharpExpressionFinder(ParserService.GetParseInformation(fileName));
		}
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
			ExpressionContext context = null;
			if (ch == '(') {
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
							if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ParserService.CurrentProjectContent.GetClass("System.MulticastDelegate", 0))) {
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
			}
			
			if ((char.IsLetter(ch) || ch == '_') && CodeCompletionOptions.CompleteWhenTyping) {
				if (cursor > 1 && !char.IsLetterOrDigit(editor.Document.GetCharAt(cursor - 1))
				    && !IsInComment(editor))
				{
					ExpressionResult result = ef.FindExpression(editor.Text, cursor);
					LoggingService.Debug("CC: Beginning to type a word, result=" + result);
					if (result.Context != ExpressionContext.IdentifierExpected) {
						editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(result.Context), '\0');
					}
				}
			}
			
			return base.HandleKeyPress(editor, ch);
		}
		
		bool IsInComment(SharpDevelopTextAreaControl editor)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset - 1;
			return ef.FilterComments(editor.Document.GetText(0, cursor + 1), ref cursor) == null;
		}
		
		public override bool HandleKeyword(SharpDevelopTextAreaControl editor, string word)
		{
			switch (word) {
				case "using":
					if (IsInComment(editor)) return false;
					
					ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
					if (parseInfo != null) {
						IClass innerMostClass = parseInfo.MostRecentCompilationUnit.GetInnermostClass(editor.ActiveTextAreaControl.Caret.Line + 1, editor.ActiveTextAreaControl.Caret.Column + 1);
						if (innerMostClass == null) {
							editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(ExpressionContext.Namespace), ' ');
						}
						return true;
					}
					return false;
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
					if (IsInComment(editor)) return false;
					return DoCaseCompletion(editor);
				case "return":
					if (IsInComment(editor)) return false;
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
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
			ExpressionResult expressionResult = ef.FindExpression(editor.Document.GetText(0, cursor), cursor);
			LoggingService.Debug("ShowNewCompletion: expression is " + expressionResult);
			if (expressionResult.Context.IsObjectCreation) {
				editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(expressionResult.Context), ' ');
				return true;
			}
			return false;
		}
		
		#region "case"-keyword completion
		bool DoCaseCompletion(SharpDevelopTextAreaControl editor)
		{
			ICSharpCode.TextEditor.Caret caret = editor.ActiveTextAreaControl.Caret;
			NRefactoryResolver r = new NRefactoryResolver(LanguageProperties.CSharp);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), caret.Line + 1, caret.Column + 1)) {
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
