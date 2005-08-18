// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace CSharpBinding
{
	public class CSharpCompletionBinding : DefaultCodeCompletionBinding
	{
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			Parser.ExpressionFinder ef = new Parser.ExpressionFinder(editor.FileName);
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
							context = ExpressionContext.TypeDerivingFrom(ReflectionReturnType.Disposable.GetUnderlyingClass(), false);
							break;
						case "catch":
							context = ExpressionContext.TypeDerivingFrom(ReflectionReturnType.Exception.GetUnderlyingClass(), false);
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
					editor.ShowCompletionWindow(new AttributesDataProvider(), ch);
					return true;
				}
			} else if (ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled) {
				// Show MethodInsightWindow or IndexerInsightWindow
				string documentText = editor.Text;
				int oldCursor = cursor;
				string textWithoutComments = ef.FilterComments(documentText, ref cursor);
				int commentLength = oldCursor - cursor;
				if (textWithoutComments != null) {
					Stack<ResolveResult> parameters = new Stack<ResolveResult>();
					char c = '\0';
					while (cursor > 0) {
						while (--cursor > 0 &&
						       ((c = textWithoutComments[cursor]) == ',' ||
						        char.IsWhiteSpace(c)));
						if (c == '(') {
							ShowInsight(editor, new MethodInsightDataProvider(cursor + commentLength, true), parameters, ch);
							return true;
						} else if (c == '[') {
							ShowInsight(editor, new IndexerInsightDataProvider(cursor + commentLength, true), parameters, ch);
							return true;
						}
						string expr = ef.FindExpressionInternal(textWithoutComments, cursor);
						if (expr == null || expr.Length == 0)
							break;
						parameters.Push(ParserService.Resolve(new ExpressionResult(expr),
						                                      editor.ActiveTextAreaControl.Caret.Line,
						                                      editor.ActiveTextAreaControl.Caret.Column,
						                                      editor.FileName,
						                                      documentText));
						cursor = ef.LastExpressionStartPosition;
					}
				}
			} else if(ch == '=') {
				LineSegment curLine = editor.Document.GetLineSegmentForOffset(cursor);
				string documentText = editor.Text;
				int position = editor.ActiveTextAreaControl.Caret.Offset - 2;
				
				if (position > 0 && (documentText[position + 1] == '+' || documentText[position + 1] == '-')) {
					ExpressionResult result = ef.FindFullExpression(documentText, position);
					
					if(result.Expression != null) {
						ResolveResult resolveResult = ParserService.Resolve(result, editor.ActiveTextAreaControl.Caret.Line, editor.ActiveTextAreaControl.Caret.Column, editor.FileName, documentText);
						if (resolveResult.ResolvedType.GetUnderlyingClass().IsTypeInInheritanceTree(ProjectContentRegistry.Mscorlib.GetClass("System.MulticastDelegate"))) {
							EventHandlerCompletitionDataProvider eventHandlerProvider = new EventHandlerCompletitionDataProvider(result.Expression, resolveResult);
							eventHandlerProvider.InsertSpace = true;
							editor.ShowCompletionWindow(eventHandlerProvider, ch);
						}
					}
				}
			}
			
			return base.HandleKeyPress(editor, ch);
		}
		
		void ShowInsight(SharpDevelopTextAreaControl editor, MethodInsightDataProvider dp, Stack<ResolveResult> parameters, char charTyped)
		{
			int paramCount = parameters.Count;
			dp.SetupDataProvider(editor.FileName, editor.ActiveTextAreaControl.TextArea);
			List<IMethodOrIndexer> methods = dp.Methods;
			if (methods.Count == 0) return;
			bool overloadIsSure;
			if (methods.Count == 1) {
				overloadIsSure = true;
				dp.DefaultIndex = 0;
			} else {
				IReturnType[] parameterTypes = new IReturnType[paramCount + 1];
				for (int i = 0; i < paramCount; i++) {
					ResolveResult rr = parameters.Pop();
					if (rr != null) {
						parameterTypes[i] = rr.ResolvedType;
					}
				}
				dp.DefaultIndex = TypeVisitor.FindOverload(methods, parameterTypes, false, out overloadIsSure);
			}
			editor.ShowInsightWindow(dp);
			if (overloadIsSure) {
				IMethodOrIndexer method = methods[dp.DefaultIndex];
				if (paramCount < method.Parameters.Count) {
					IParameter param = method.Parameters[paramCount];
					ProvideContextCompletion(editor, param.ReturnType, charTyped);
				}
			}
		}
		
		void ProvideContextCompletion(SharpDevelopTextAreaControl editor, IReturnType expected, char charTyped)
		{
			IClass c = expected.GetUnderlyingClass();
			if (c == null) return;
			if (c.ClassType == ClassType.Enum) {
				CtrlSpaceCompletionDataProvider cdp = new CtrlSpaceCompletionDataProvider();
				cdp.ForceNewExpression = true;
				CachedCompletionDataProvider cache = new CachedCompletionDataProvider(cdp);
				cache.GenerateCompletionData(editor.FileName, editor.ActiveTextAreaControl.TextArea, charTyped);
				ICompletionData[] completionData = cache.CompletionData;
				Array.Sort(completionData);
				for (int i = 0; i < completionData.Length; i++) {
					CodeCompletionData ccd = completionData[i] as CodeCompletionData;
					if (ccd != null && ccd.Class != null) {
						if (ccd.Class.FullyQualifiedName == expected.FullyQualifiedName) {
							cache.DefaultIndex = i;
							break;
						}
					}
				}
				if (cache.DefaultIndex >= 0) {
					cache.InsertSpace = true;
					editor.ShowCompletionWindow(cache, charTyped);
				}
			}
		}
		
		bool IsInComment(SharpDevelopTextAreaControl editor)
		{
			Parser.ExpressionFinder ef = new Parser.ExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
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
					// TODO: Suggest list of virtual methods to override
					return false;
				case "new":
					return ShowNewCompletion(editor);
				default:
					return base.HandleKeyword(editor, word);
			}
		}
		
		bool ShowNewCompletion(SharpDevelopTextAreaControl editor)
		{
			Parser.ExpressionFinder ef = new Parser.ExpressionFinder(editor.FileName);
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
			ExpressionContext context = ef.FindExpression(editor.Document.GetText(0, cursor) + " T.", cursor + 2).Context;
			if (context.IsObjectCreation) {
				editor.ShowCompletionWindow(new CtrlSpaceCompletionDataProvider(context), ' ');
				return true;
			}
			return false;
		}
	}
}
