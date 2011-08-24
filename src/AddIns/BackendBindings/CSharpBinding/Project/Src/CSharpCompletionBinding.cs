// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using AST = ICSharpCode.NRefactory.Ast;

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
		
		public override CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			int cursor = editor.Caret.Offset;
			if (ch == '[') {
				var line = editor.Document.GetLineForOffset(cursor);
				/* TODO: AVALONEDIT Reimplement this
				if (TextUtilities.FindPrevWordStart(editor.ActiveTextAreaControl.Document, cursor) <= line.Offset) {
					// [ is first character on the line
					// -> Attribute completion
					editor.ShowCompletionWindow(new AttributesDataProvider(ParserService.CurrentProjectContent), ch);
					return true;
				}*/
			} else if (ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled) {
				IInsightWindow insightWindow;
				if (insightHandler.InsightRefreshOnComma(editor, ch, out insightWindow)) {
					return CodeCompletionKeyPressResult.Completed;
				}
			} else if(ch == '=') {
				var curLine = editor.Document.GetLineForOffset(cursor);
				string documentText = editor.Document.Text;
				int position = editor.Caret.Offset - 2;
				
				if (position > 0 && (documentText[position + 1] == '+')) {
					ExpressionResult result = ef.FindFullExpression(documentText, position);
					
					if(result.Expression != null) {
						ResolveResult resolveResult = ParserService.Resolve(result, editor.Caret.Line, editor.Caret.Column, editor.FileName, documentText);
						if (resolveResult != null && resolveResult.ResolvedType != null) {
							IClass underlyingClass = resolveResult.ResolvedType.GetUnderlyingClass();
							if (underlyingClass != null && underlyingClass.IsTypeInInheritanceTree(ParserService.CurrentProjectContent.GetClass("System.MulticastDelegate", 0))) {
								EventHandlerCompletionItemProvider eventHandlerProvider = new EventHandlerCompletionItemProvider(result.Expression, resolveResult);
								eventHandlerProvider.ShowCompletion(editor);
								return CodeCompletionKeyPressResult.Completed;
							}
						}
					}
				} else if (position > 0) {
					ExpressionResult result = ef.FindFullExpression(documentText, position);
					
					if(result.Expression != null) {
						ResolveResult resolveResult = ParserService.Resolve(result, editor.Caret.Line, editor.Caret.Column, editor.FileName, documentText);
						if (resolveResult != null && resolveResult.ResolvedType != null) {
							if (ProvideContextCompletion(editor, resolveResult.ResolvedType, ch)) {
								return CodeCompletionKeyPressResult.Completed;
							}
						}
					}
				}
			} else if (ch == '.') {
				new CSharpCodeCompletionDataProvider().ShowCompletion(editor);
				return CodeCompletionKeyPressResult.Completed;
			} else if (ch == '>') {
				if (IsInComment(editor)) return CodeCompletionKeyPressResult.None;
				char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
				if (prevChar == '-') {
					new PointerArrowCompletionDataProvider().ShowCompletion(editor);
					
					return CodeCompletionKeyPressResult.Completed;
				}
			}
			
			if (char.IsLetter(ch) && CodeCompletionOptions.CompleteWhenTyping) {
				if (editor.SelectionLength > 0) {
					// allow code completion when overwriting an identifier
					int endOffset = editor.SelectionStart + editor.SelectionLength;
					// but block code completion when overwriting only part of an identifier
					if (endOffset < editor.Document.TextLength && char.IsLetterOrDigit(editor.Document.GetCharAt(endOffset)))
						return CodeCompletionKeyPressResult.None;
					
					editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
					// Read caret position again after removal - this is required because the document might change in other
					// locations, too (e.g. bound elements in snippets).
					cursor = editor.Caret.Offset;
				}
				char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
				bool afterUnderscore = prevChar == '_';
				if (afterUnderscore) {
					cursor--;
					prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
				}
				if (!char.IsLetterOrDigit(prevChar) && prevChar != '.' && !IsInComment(editor)) {
					ExpressionResult result = ef.FindExpression(editor.Document.Text, cursor);
					LoggingService.Debug("CC: Beginning to type a word, result=" + result);
					if (result.Context != ExpressionContext.IdentifierExpected) {
						var ctrlSpaceProvider = new NRefactoryCtrlSpaceCompletionItemProvider(LanguageProperties.CSharp, result.Context);
						ctrlSpaceProvider.ShowTemplates = true;
						ctrlSpaceProvider.AllowCompleteExistingExpression = afterUnderscore;
						ctrlSpaceProvider.ShowCompletion(editor);
						return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
					}
				}
			}
			
			return base.HandleKeyPress(editor, ch);
		}
		
		class CSharpCodeCompletionDataProvider : DotCodeCompletionItemProvider
		{
			public override ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
			{
				// bypass ParserService.Resolve and set resolver.LimitMethodExtractionUntilCaretLine
				ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
				NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
				resolver.LimitMethodExtractionUntilLine = editor.Caret.Line;
				return resolver.Resolve(expressionResult, parseInfo, editor.Document.Text);
			}
		}
		
		class PointerArrowCompletionDataProvider : DotCodeCompletionItemProvider
		{
			public override ResolveResult Resolve(ITextEditor editor, ExpressionResult expressionResult)
			{
				ResolveResult rr = base.Resolve(editor, expressionResult);
				if (rr != null && rr.ResolvedType != null) {
					PointerReturnType prt = rr.ResolvedType.CastToDecoratingReturnType<PointerReturnType>();
					if (prt != null)
						return new ResolveResult(rr.CallingClass, rr.CallingMember, prt.BaseType);
				}
				return null;
			}
			
			public override ExpressionResult GetExpression(ITextEditor editor)
			{
				// - 1 because the "-" is already inserted (the ">" is about to be inserted)
				return GetExpressionFromOffset(editor, editor.Caret.Offset - 1);
			}
		}
		
		bool IsInComment(ITextEditor editor)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			int cursor = editor.Caret.Offset - 1;
			return ef.FilterComments(editor.Document.GetText(0, cursor + 1), ref cursor) == null;
		}
		
		public override bool HandleKeyword(ITextEditor editor, string word)
		{
			switch (word) {
				case "using":
					if (IsInComment(editor)) return false;
					
					ParseInformation parseInfo = ParserService.GetParseInformation(editor.FileName);
					if (parseInfo != null) {
						IClass innerMostClass = parseInfo.CompilationUnit.GetInnermostClass(editor.Caret.Line, editor.Caret.Column);
						if (innerMostClass == null) {
							new NRefactoryCtrlSpaceCompletionItemProvider(LanguageProperties.CSharp, ExpressionContext.Namespace).ShowCompletion(editor);
							return true;
						}
					}
					break;
				case "as":
				case "is":
					if (IsInComment(editor)) return false;
					new NRefactoryCtrlSpaceCompletionItemProvider(LanguageProperties.CSharp, ExpressionContext.Type).ShowCompletion(editor);
					return true;
				case "override":
					if (IsInComment(editor)) return false;
					new OverrideCompletionItemProvider().ShowCompletion(editor);
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
					}
					break;
			}
			return base.HandleKeyword(editor, word);
		}
		
		bool ShowNewCompletion(ITextEditor editor)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			int cursor = editor.Caret.Offset;
			string documentToCursor = editor.Document.GetText(0, cursor);
			ExpressionResult expressionResult = ef.FindExpression(documentToCursor, cursor);
			
			LoggingService.Debug("ShowNewCompletion: expression is " + expressionResult);
			if (expressionResult.Context.IsObjectCreation) {
				var currentLine = editor.Document.GetLineForOffset(cursor);
				string lineText = editor.Document.GetText(currentLine.Offset, cursor - currentLine.Offset);
				// when the new follows an assignment, improve code-completion by detecting the
				// type of the variable that is assigned to
				if (lineText.Replace(" ", "").EndsWith("=new")) {
					int pos = lineText.LastIndexOf('=');
					ExpressionContext context = FindExactContextForNewCompletion(editor, documentToCursor,
					                                                             currentLine, pos);
					if (context != null)
						expressionResult.Context = context;
				}
				new NRefactoryCtrlSpaceCompletionItemProvider(LanguageProperties.CSharp, expressionResult.Context).ShowCompletion(editor);
				return true;
			}
			return false;
		}
		
		ExpressionContext FindExactContextForNewCompletion(ITextEditor editor, string documentToCursor,
		                                                   IDocumentLine currentLine, int pos)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			// find expression on left hand side of the assignment
			ExpressionResult lhsExpr = ef.FindExpression(documentToCursor, currentLine.Offset + pos);
			if (lhsExpr.Expression != null) {
				ResolveResult rr = ParserService.Resolve(lhsExpr, currentLine.LineNumber, pos, editor.FileName, editor.Document.Text);
				if (rr != null && rr.ResolvedType != null) {
					ExpressionContext context;
					IClass c;
					if (rr.ResolvedType.IsArrayReturnType) {
						// when creating an array, all classes deriving from the array's element type are allowed
						IReturnType elementType = rr.ResolvedType.CastToArrayReturnType().ArrayElementType;
						c = elementType != null ? elementType.GetUnderlyingClass() : null;
						context = ExpressionContext.TypeDerivingFrom(elementType, false);
					} else {
						// when creating a normal instance, all non-abstract classes deriving from the type
						// are allowed
						c = rr.ResolvedType.GetUnderlyingClass();
						context = ExpressionContext.TypeDerivingFrom(rr.ResolvedType, true);
					}
					if (c != null && context.ShowEntry(c)) {
						// Try to suggest an entry (List<int> a = new => suggest List<int>).
						
						string suggestedClassName = LanguageProperties.CSharp.CodeGenerator.GenerateCode(
							CodeGenerator.ConvertType(
								rr.ResolvedType,
								new ClassFinder(ParserService.GetParseInformation(editor.FileName), editor.Caret.Line, editor.Caret.Column)
							), "");
						if (suggestedClassName != c.Name) {
							// create a special code completion item that completes also the type arguments
							context.SuggestedItem = new SuggestedCodeCompletionItem(c, suggestedClassName);
						} else {
							context.SuggestedItem = new CodeCompletionItem(c);
						}
					}
					return context;
				}
			}
			return null;
		}
		
		#region "case"-keyword completion
		bool DoCaseCompletion(ITextEditor editor)
		{
			ITextEditorCaret caret = editor.Caret;
			NRefactoryResolver r = new NRefactoryResolver(LanguageProperties.CSharp);
			if (r.Initialize(ParserService.GetParseInformation(editor.FileName), caret.Line, caret.Column)) {
				AST.INode currentMember = r.ParseCurrentMember(editor.Document.Text);
				if (currentMember != null) {
					CaseCompletionSwitchFinder ccsf = new CaseCompletionSwitchFinder(caret.Line, caret.Column);
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
