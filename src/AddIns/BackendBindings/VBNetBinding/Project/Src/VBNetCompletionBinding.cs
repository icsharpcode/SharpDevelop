// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
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
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.TextEditor;
using VBTokens = ICSharpCode.NRefactory.Parser.VB.Tokens;

namespace VBNetBinding
{
	public class VBNetCompletionBinding : DefaultCodeCompletionBinding
	{
		public VBNetCompletionBinding()
		{
			// Don't use indexer insight for '[', VB uses '(' for indexer access
			this.EnableIndexerInsight = false;
		}
		
		public override bool HandleKeyPress(SharpDevelopTextAreaControl editor, char ch)
		{
			VBNetBinding.Parser.ExpressionFinder ef = new VBNetBinding.Parser.ExpressionFinder();
			int cursor = editor.ActiveTextAreaControl.Caret.Offset;
			
			if(ch == '(' && EnableMethodInsight && CodeCompletionOptions.InsightEnabled)
			{
				editor.ShowInsightWindow(new MethodInsightDataProvider());
				return true;
			}
			else if(ch == ',' && CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled)
			{
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
						if (c == '(')
						{
							ShowInsight(editor, new MethodInsightDataProvider(cursor + commentLength, true), parameters, ch);
							return true;
						}
						else if (c == '[')
						{
							ShowInsight(editor, new IndexerInsightDataProvider(cursor + commentLength, true), parameters, ch);
							return true;
						}
						string expr = ef.FindExpressionInternal(textWithoutComments, cursor);
						if (expr == null || expr.Length == 0)
						{
							break;
						}
						parameters.Push(ParserService.Resolve(new ExpressionResult(expr),
						                                      editor.ActiveTextAreaControl.Caret.Line + 1,
						                                      editor.ActiveTextAreaControl.Caret.Column + 1,
						                                      editor.FileName,
						                                      documentText));
						cursor = ef.LastExpressionStartPosition;
					}
				}
			} else if (ch == '\n') {
				TryDeclarationTypeInference(editor, editor.Document.GetLineSegmentForOffset(cursor));
			}
			return base.HandleKeyPress(editor, ch);
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
					editor.ShowCompletionWindow(cache, charTyped);
				}
			}
		}
		
		void ShowInsight(SharpDevelopTextAreaControl editor, MethodInsightDataProvider dp, Stack<ResolveResult> parameters, char charTyped)
		{
			int paramCount = parameters.Count;
			dp.SetupDataProvider(editor.FileName, editor.ActiveTextAreaControl.TextArea);
			List<IMethodOrProperty> methods = dp.Methods;
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
				IReturnType[][] tmp;
				int[] ranking = MemberLookupHelper.RankOverloads(methods, parameterTypes, true, out overloadIsSure, out tmp);
				bool multipleBest = false;
				int bestRanking = -1;
				int best = 0;
				for (int i = 0; i < ranking.Length; i++) {
					if (ranking[i] > bestRanking) {
						bestRanking = ranking[i];
						best = i;
						multipleBest = false;
					} else if (ranking[i] == bestRanking) {
						multipleBest = true;
					}
				}
				if (multipleBest) overloadIsSure = false;
				dp.DefaultIndex = best;
			}
			editor.ShowInsightWindow(dp);
			if (overloadIsSure) {
				IMethodOrProperty method = methods[dp.DefaultIndex];
				if (paramCount < method.Parameters.Count) {
					IParameter param = method.Parameters[paramCount];
					ProvideContextCompletion(editor, param.ReturnType, charTyped);
				}
			}
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
				if (ICSharpCode.SharpDevelop.Refactoring.CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
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
