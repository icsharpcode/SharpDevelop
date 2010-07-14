// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.VBNet;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.VBNetBinding
{
	public class VBNetCompletionBinding : ICodeCompletionBinding
	{
		static VBNetCompletionBinding instance;
		
		public static VBNetCompletionBinding Instance {
			get {
				if (instance == null)
					instance = new VBNetCompletionBinding();
				return instance;
			}
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (IsInComment(editor) || IsInString(editor))
				return CodeCompletionKeyPressResult.None;
			
			if (editor.SelectionLength > 0) {
				// allow code completion when overwriting an identifier
				int endOffset = editor.SelectionStart + editor.SelectionLength;
				// but block code completion when overwriting only part of an identifier
				if (endOffset < editor.Document.TextLength && char.IsLetterOrDigit(editor.Document.GetCharAt(endOffset)))
					return CodeCompletionKeyPressResult.None;
				
				editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
			}
			
			if (ch == ' ') {

			}
			
			VBNetExpressionFinder ef = new VBNetExpressionFinder(ParserService.GetParseInformation(editor.FileName));
			
			ExpressionResult result;
			
			switch (ch) {
				case '(':
					if (CodeCompletionOptions.InsightEnabled) {
						IInsightWindow insightWindow = editor.ShowInsightWindow(new MethodInsightProvider().ProvideInsight(editor));
//						if (insightWindow != null)
//							InitializeOpenedInsightWindow(editor, insightWindow);
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '.':
					result = ef.FindExpression(editor.Document.Text, editor.Caret.Offset);
					LoggingService.Debug("CC: After dot, result=" + result + ", context=" + result.Context);
					editor.ShowCompletionWindow(CompletionDataHelper.GenerateCompletionData(result, editor, ch));
					return CodeCompletionKeyPressResult.Completed;
				case ' ':
					editor.Document.Insert(editor.Caret.Offset, " ");
					result = ef.FindExpression(editor.Document.Text, editor.Caret.Offset);
					if (!LiteralMayFollow((BitArray)result.Tag) && !OperatorMayFollow((BitArray)result.Tag) && ExpressionContext.IdentifierExpected != result.Context) {
						LoggingService.Debug("CC: After space, result=" + result + ", context=" + result.Context);
						editor.ShowCompletionWindow(CompletionDataHelper.GenerateCompletionData(result, editor, ch));
					}
					return CodeCompletionKeyPressResult.EatKey;
				default:
					if (CodeCompletionOptions.CompleteWhenTyping) {
						int cursor = editor.Caret.Offset;
						char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
						bool afterUnderscore = prevChar == '_';
						if (afterUnderscore) {
							cursor--;
							prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
						}
						
						result = ef.FindExpression(editor.Document.Text, cursor);
						
						if ((result.Context != ExpressionContext.IdentifierExpected && char.IsLetter(ch)) &&
						    (!char.IsLetterOrDigit(prevChar) && prevChar != '.')) {
							LoggingService.Debug("CC: Beginning to type a word, result=" + result + ", context=" + result.Context);
							editor.ShowCompletionWindow(CompletionDataHelper.GenerateCompletionData(result, editor, ch));
							return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		bool OperatorMayFollow(BitArray array)
		{
			if (array == null)
				return false;
			
			return array[Tokens.Xor];
		}
		
		bool LiteralMayFollow(BitArray array)
		{
			if (array == null)
				return false;
			
			for (int i = 0; i < array.Length; i++) {
				if (array[i] && i >= Tokens.LiteralString && i <= Tokens.LiteralDate)
					return true;
			}
			
			return false;
		}
		
		bool HasKeywordsOnly(BitArray array)
		{
			if (array == null)
				return false;
			
			for (int i = 0; i < array.Length; i++) {
				if (array[i] && i < Tokens.AddHandler)
					return false;
			}
			
			return true;
		}
		
		void GetCommentOrStringState(ITextEditor editor, out bool inString, out bool inComment)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, editor.Document.CreateReader());
			
			Token t = lexer.NextToken();
			bool inXml = false;
			
			inString = false;
			inComment = false;
			
			while (t.Location < editor.Caret.Position) {
				t = lexer.NextToken();
				
				if (inXml && (t.Kind != Tokens.Identifier && t.Kind != Tokens.LiteralString && !(t.Kind > Tokens.LiteralDate && t.Kind < Tokens.Colon))) {
					inXml = false;
					continue;
				}
				
				if (t.Kind > Tokens.LiteralDate && t.Kind < Tokens.Assign)
					inXml = true;
			}
			
			if (inXml) {
				// TODO
			} else {
				string lineText = editor.Document.GetLine(editor.Caret.Line).Text;
				
				for (int i = 0; i < editor.Caret.Column - 1; i++) {
					char ch = lineText[i];
					
					if (!inComment && ch == '"')
						inString = !inString;
					if (!inString && ch == '\'')
						inComment = true;
				}
			}
		}
		
		bool IsInString(ITextEditor editor)
		{
			bool inString, inComment;
			GetCommentOrStringState(editor, out inString, out inComment);
			return inString;
		}
		
		bool IsInComment(ITextEditor editor)
		{
			bool inString, inComment;
			GetCommentOrStringState(editor, out inString, out inComment);
			return inComment;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			if (IsInComment(editor))
				return false;
			
			if (editor.SelectionLength > 0) {
				// allow code completion when overwriting an identifier
				int endOffset = editor.SelectionStart + editor.SelectionLength;
				// but block code completion when overwriting only part of an identifier
				if (endOffset < editor.Document.TextLength && char.IsLetterOrDigit(editor.Document.GetCharAt(endOffset)))
					return false;
				
				editor.Document.Remove(editor.SelectionStart, editor.SelectionLength);
			}
			
			int cursor = editor.Caret.Offset;
			char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
			bool afterUnderscore = prevChar == '_';
			
			if (afterUnderscore) {
				cursor--;
				prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
			}
			
			if (!char.IsLetterOrDigit(prevChar) && prevChar != '.') {
				VBNetExpressionFinder ef = new VBNetExpressionFinder(ParserService.GetParseInformation(editor.FileName));
				ExpressionResult result = ef.FindExpression(editor.Document.Text, cursor);
				LoggingService.Debug("CC: Beginning to type a word, result=" + result + ", context=" + result.Context);
				if (result.Context != ExpressionContext.IdentifierExpected) {
					editor.ShowCompletionWindow(CompletionDataHelper.GenerateCompletionData(result, editor, ' '));
					return true;
				}
			}
			
			return false;
		}
	}
}
