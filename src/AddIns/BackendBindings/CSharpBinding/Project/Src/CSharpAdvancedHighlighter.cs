// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace CSharpBinding
{
	public class CSharpAdvancedHighlighter : AsynchronousAdvancedHighlighter
	{
		public override void Initialize(TextEditorControl textEditor)
		{
			base.Initialize(textEditor);
			ParserService.ParserUpdateStepFinished += OnUpdateStep;
		}
		
		public override void Dispose()
		{
			ParserService.ParserUpdateStepFinished -= OnUpdateStep;
			base.Dispose();
		}
		
		void OnUpdateStep(object sender, ParserUpdateStepEventArgs e)
		{
			if (FileUtility.IsEqualFileName(e.FileName, this.TextEditor.FileName)) {
				ParseInformation parseInfo = e.ParseInformation;
//				if (parseInfo == null && this.storedParseInformation)
//					parseInfo = ParserService.GetParseInformation(this.TextEditor.FileName);
//				if (parseInfo != null) {
//					ICompilationUnit cu = parseInfo.MostRecentCompilationUnit;
//				}
				WorkbenchSingleton.SafeThreadAsyncCall(MarkOutstanding);
			}
		}
		
		static bool IsInMultilineCommentOrStringLiteral(LineSegment line)
		{
			if (line.HighlightSpanStack == null || line.HighlightSpanStack.IsEmpty) {
				return false;
			}
			return !line.HighlightSpanStack.Peek().StopEOL;
		}
		
		protected override void MarkWords(int lineNumber, LineSegment currentLine, List<TextWord> words)
		{
			if (IsInMultilineCommentOrStringLiteral(currentLine)) {
				return;
			}
			ParseInformation parseInfo = ParserService.GetParseInformation(this.TextEditor.FileName);
			if (parseInfo == null) return;
			
			CSharpExpressionFinder finder = new CSharpExpressionFinder(parseInfo);
			Func<string, int, ExpressionResult> findExpressionMethod;
			IClass callingClass = parseInfo.MostRecentCompilationUnit.GetInnermostClass(lineNumber, 0);
			if (callingClass != null) {
				if (GetCurrentMember(callingClass, lineNumber, 0) != null) {
					findExpressionMethod = finder.FindFullExpressionInMethod;
				} else {
					findExpressionMethod = finder.FindFullExpressionInTypeDeclaration;
				}
			} else {
				findExpressionMethod = finder.FindFullExpression;
			}
			
			string lineText = this.Document.GetText(currentLine.Offset, currentLine.Length);
			bool changedLine = false;
			// now go through the word list:
			foreach (TextWord word in words) {
				if (word.IsWhiteSpace) continue;
				if (char.IsLetter(lineText[word.Offset]) || lineText[word.Offset] == '_') {
					ExpressionResult result = findExpressionMethod(lineText, word.Offset);
					if (result.Expression != null) {
						// result.Expression
						if (ICSharpCode.NRefactory.Parser.CSharp.Keywords.IsNonIdentifierKeyword(result.Expression))
							continue;
						// convert text editor to DOM coordinates:
						resolveCount++;
						ResolveResult rr = ParserService.Resolve(result, lineNumber + 1, word.Offset + 1, this.TextEditor.FileName, this.TextEditor.Text);
						if (rr is MixedResolveResult || rr is TypeResolveResult) {
							changedLine = true;
							word.SyntaxColor = this.Document.HighlightingStrategy.GetColorFor("TypeReference");
						} else if (rr == null) {
							changedLine = true;
							word.SyntaxColor = this.Document.HighlightingStrategy.GetColorFor("UnknownEntity");
						}
					}
				}
			}
			
			if (markingOutstanding && changedLine) {
				this.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineNumber));
			}
		}
		
		static IMember GetCurrentMember(IClass callingClass, int caretLine, int caretColumn)
		{
			if (callingClass == null)
				return null;
			foreach (IMethod method in callingClass.Methods) {
				if (method.Region.IsInside(caretLine, caretColumn) || method.BodyRegion.IsInside(caretLine, caretColumn)) {
					return method;
				}
			}
			foreach (IProperty property in callingClass.Properties) {
				if (property.Region.IsInside(caretLine, caretColumn) || property.BodyRegion.IsInside(caretLine, caretColumn)) {
					return property;
				}
			}
			return null;
		}
		
		bool markingOutstanding;
		int resolveCount;
		
		protected override void MarkOutstanding()
		{
			#if DEBUG
			int time = Environment.TickCount;
			#endif
			markingOutstanding = true;
			resolveCount = 0;
			base.MarkOutstanding();
			markingOutstanding = false;
			#if DEBUG
			time = Environment.TickCount - time;
			if (time > 0) {
				LoggingService.Info("CSharpHighlighter took " + time + "ms for " + resolveCount + " resolves");
			}
			#endif
			this.Document.CommitUpdate();
		}
	}
}
*/
