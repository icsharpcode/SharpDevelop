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
			} else if (ch == '.') {
				editor.ShowCompletionWindow(new CSharpCodeCompletionDataProvider(), ch);
				return true;
			} else if (ch == '>') {
				if (IsInComment(editor)) return false;
				char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
				if (prevChar == '-') {
					editor.ShowCompletionWindow(new PointerArrowCompletionDataProvider(), ch);
					
					return true;
				}
			}
			
			if (char.IsLetter(ch) && CodeCompletionOptions.CompleteWhenTyping) {
				if (editor.ActiveTextAreaControl.SelectionManager.HasSomethingSelected) {
					// allow code completion when overwriting an identifier
					cursor = editor.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].Offset;
					int endOffset = editor.ActiveTextAreaControl.SelectionManager.SelectionCollection[0].EndOffset;
					// but block code completion when overwriting only part of an identifier
					if (endOffset < editor.Document.TextLength && char.IsLetterOrDigit(editor.Document.GetCharAt(endOffset)))
						return false;
					editor.ActiveTextAreaControl.SelectionManager.RemoveSelectedText();
					editor.ActiveTextAreaControl.Caret.Position = editor.Document.OffsetToPosition(cursor);
				}
				char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
				bool afterUnderscore = prevChar == '_';
				if (afterUnderscore) {
					cursor--;
					prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
				}
				if (!char.IsLetterOrDigit(prevChar) && prevChar != '.' && !IsInComment(editor)) {
					ExpressionResult result = ef.FindExpression(editor.Text, cursor);
					LoggingService.Debug("CC: Beginning to type a word, result=" + result);
					if (result.Context != ExpressionContext.IdentifierExpected) {
						editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(result.Context) {
						                            	ShowTemplates = true,
						                            	AllowCompleteExistingExpression = afterUnderscore
						                            }, '\0');
					}
				}
			}
			
			return base.HandleKeyPress(editor, ch);
		}
		
		class CSharpCodeCompletionDataProvider : CodeCompletionDataProvider
		{
			protected override ResolveResult Resolve(ExpressionResult expressionResult, int caretLineNumber, int caretColumn, string fileName, string fileContent)
			{
				// bypass ParserService.Resolve and set resolver.LimitMethodExtractionUntilCaretLine
				ParseInformation parseInfo = ParserService.GetParseInformation(fileName);
				NRefactoryResolver resolver = new NRefactoryResolver(LanguageProperties.CSharp);
				resolver.LimitMethodExtractionUntilLine = caretLineNumber;
				return resolver.Resolve(expressionResult, parseInfo, fileContent);
			}
		}
		
		class PointerArrowCompletionDataProvider : CodeCompletionDataProvider
		{
			protected override ResolveResult Resolve(ExpressionResult expressionResult, int caretLineNumber, int caretColumn, string fileName, string fileContent)
			{
				ResolveResult rr = base.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, fileContent);
				if (rr != null && rr.ResolvedType != null) {
					PointerReturnType prt = rr.ResolvedType.CastToDecoratingReturnType<PointerReturnType>();
					if (prt != null)
						return new ResolveResult(rr.CallingClass, rr.CallingMember, prt.BaseType);
				}
				return null;
			}
			
			protected override ExpressionResult GetExpression(ICSharpCode.TextEditor.TextArea textArea)
			{
				ICSharpCode.TextEditor.Document.IDocument document = textArea.Document;
				IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
				if (expressionFinder == null) {
					return new ExpressionResult(TextUtilities.GetExpressionBeforeOffset(textArea, textArea.Caret.Offset - 1));
				} else {
					ExpressionResult res = expressionFinder.FindExpression(document.GetText(0, textArea.Caret.Offset - 1), textArea.Caret.Offset - 1);
					if (overrideContext != null)
						res.Context = overrideContext;
					return res;
				}
			}
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
			string documentToCursor = editor.Document.GetText(0, cursor);
			ExpressionResult expressionResult = ef.FindExpression(documentToCursor, cursor);
			
			LoggingService.Debug("ShowNewCompletion: expression is " + expressionResult);
			if (expressionResult.Context.IsObjectCreation) {
				LineSegment currentLine = editor.Document.GetLineSegmentForOffset(cursor);
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
				editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(expressionResult.Context), ' ');
				return true;
			}
			return false;
		}
		
		ExpressionContext FindExactContextForNewCompletion(SharpDevelopTextAreaControl editor, string documentToCursor,
		                                                   LineSegment currentLine, int pos)
		{
			CSharpExpressionFinder ef = CreateExpressionFinder(editor.FileName);
			// find expression on left hand side of the assignment
			ExpressionResult lhsExpr = ef.FindExpression(documentToCursor, currentLine.Offset + pos);
			if (lhsExpr.Expression != null) {
				ResolveResult rr = ParserService.Resolve(lhsExpr, currentLine.LineNumber, pos, editor.FileName, editor.Text);
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
								new ClassFinder(ParserService.GetParseInformation(editor.FileName), editor.ActiveTextAreaControl.Caret.Line + 1, editor.ActiveTextAreaControl.Caret.Column + 1)
							), "");
						if (suggestedClassName != c.Name) {
							// create an IClass instance that includes the type arguments in its name
							context.SuggestedItem = new RenamedClass(c, suggestedClassName);
						} else {
							context.SuggestedItem = c;
						}
					}
					return context;
				}
			}
			return null;
		}
		
		/// <summary>
		/// A class that copies the properties important for the code completion display from another class,
		/// but provides its own Name implementation.
		/// Unlike the AbstractEntity.Name implementation, here 'Name' may include the namespace or type arguments.
		/// </summary>
		sealed class RenamedClass : DefaultClass, IClass
		{
			string newName;
			
			public RenamedClass(IClass c, string newName) : base(c.CompilationUnit, c.ClassType, c.Modifiers, c.Region, c.DeclaringType)
			{
				this.newName = newName;
				CopyDocumentationFrom(c);
				this.FullyQualifiedName = c.FullyQualifiedName;
			}
			
			string IEntity.Name {
				get { return newName; }
			}
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
