// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using Dom = ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
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
		
		NRefactoryInsightWindowHandler insightHandler = new NRefactoryInsightWindowHandler(SupportedLanguage.VBNet);
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			if (LanguageUtils.IsInsideDocumentationComment(editor) && ch == '<') {
				new CommentCompletionItemProvider().ShowCompletion(editor);
				return CodeCompletionKeyPressResult.Completed;
			}
			
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
			
			VBNetExpressionFinder ef = new VBNetExpressionFinder(ParserService.GetParseInformation(editor.FileName));
			
			ExpressionResult result;
			
			switch (ch) {
				case '(':
					if (CodeCompletionOptions.InsightEnabled) {
						IInsightWindow insightWindow = editor.ShowInsightWindow(new MethodInsightProvider().ProvideInsight(editor));
						if (insightWindow != null) {
							insightHandler.InitializeOpenedInsightWindow(editor, insightWindow);
							insightHandler.HighlightParameter(insightWindow, 0);
						}
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case ',':
					if (CodeCompletionOptions.InsightRefreshOnComma && CodeCompletionOptions.InsightEnabled) {
						IInsightWindow insightWindow;
						if (insightHandler.InsightRefreshOnComma(editor, ch, out insightWindow))
							return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '\n':
					TryDeclarationTypeInference(editor, editor.Document.GetLineForOffset(editor.Caret.Offset));
					break;
				case '.':
					string w = editor.GetWordBeforeCaret(); int index = w.IndexOf('.');
					
					if (index > -1 && w.Length - index == 2)
						index = editor.Caret.Offset - 2;
					else
						index = editor.Caret.Offset;
					
					result = ef.FindExpression(editor.Document.Text, index);
					LoggingService.Debug("CC: After dot, result=" + result + ", context=" + result.Context);
					if (ShowCompletion(result, editor, ch))
						return CodeCompletionKeyPressResult.Completed;
					else
						return CodeCompletionKeyPressResult.None;
				case '@':
					if (editor.Caret.Offset > 0 && editor.Document.GetCharAt(editor.Caret.Offset - 1) == '.')
						return CodeCompletionKeyPressResult.None;
					goto default;
				case ' ':
					editor.Document.Insert(editor.Caret.Offset, " ");
					result = ef.FindExpression(editor.Document.Text, editor.Caret.Offset);
					
					string word = editor.GetWordBeforeCaret().Trim();
					if (word.Equals("overrides", StringComparison.OrdinalIgnoreCase) || word.Equals("return", StringComparison.OrdinalIgnoreCase) || !LiteralMayFollow((BitArray)result.Tag) && !OperatorMayFollow((BitArray)result.Tag) && ExpressionContext.IdentifierExpected != result.Context) {
						LoggingService.Debug("CC: After space, result=" + result + ", context=" + result.Context);
						ShowCompletion(result, editor, ch);
					}
					return CodeCompletionKeyPressResult.EatKey;
				default:
					if (CodeCompletionOptions.CompleteWhenTyping) {
						int cursor = editor.Caret.Offset;
						char prevChar = cursor > 1 ? editor.Document.GetCharAt(cursor - 1) : ' ';
						char ppChar = cursor > 2 ? editor.Document.GetCharAt(cursor - 2) : ' ';
						
						result = ef.FindExpression(editor.Document.Text, cursor);
						
						if ((result.Context != ExpressionContext.IdentifierExpected && char.IsLetter(ch)) &&
						    (!char.IsLetterOrDigit(prevChar) && prevChar != '.')) {
							if (prevChar == '@' && ppChar == '.')
								return CodeCompletionKeyPressResult.None;
							if (IsTypeCharacter(ch, prevChar))
								return CodeCompletionKeyPressResult.None;
							if (prevChar == '_') {
								result.Expression = '_' + result.Expression;
								result.Region = new DomRegion(result.Region.BeginLine, result.Region.BeginColumn - 1, result.Region.EndLine, result.Region.EndColumn);
							}
							LoggingService.Debug("CC: Beginning to type a word, result=" + result + ", context=" + result.Context);
							ShowCompletion(result, editor, ch);
							return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		static bool IsTypeCharacter(char ch, char prevChar)
		{
			ch = char.ToUpperInvariant(ch);
			
			// char type character
			if (ch == 'C' && prevChar == '"')
				return true;
			
			// start of hex or octal literal
			if (prevChar == '&' && (ch == 'H' || ch == 'O'))
				return true;
			
			if (char.IsDigit(prevChar))
				return true;
			
			return false;
		}

		static bool ShowCompletion(ExpressionResult result, ITextEditor editor, char ch)
		{
			VBNetCompletionItemList list = CompletionDataHelper.GenerateCompletionData(result, editor, ch);
			list.Editor = editor;
			list.Window = editor.ShowCompletionWindow(list);
			return list.Items.Any();
		}
		
		#region Helpers
		static bool OperatorMayFollow(BitArray array)
		{
			if (array == null)
				return false;
			
			return array[Tokens.Xor];
		}
		
		static bool LiteralMayFollow(BitArray array)
		{
			if (array == null)
				return false;
			
			for (int i = 0; i < array.Length; i++) {
				if (array[i] && i >= Tokens.LiteralString && i <= Tokens.LiteralDate)
					return true;
			}
			
			return false;
		}
		
		static void GetCommentOrStringState(ITextEditor editor, out bool inString, out bool inComment)
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
		
		static bool IsInString(ITextEditor editor)
		{
			bool inString, inComment;
			GetCommentOrStringState(editor, out inString, out inComment);
			return inString;
		}
		
		static bool IsInComment(ITextEditor editor)
		{
			bool inString, inComment;
			GetCommentOrStringState(editor, out inString, out inComment);
			return inComment;
		}
		#endregion
		
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
			
			VBNetExpressionFinder ef = new VBNetExpressionFinder(ParserService.GetParseInformation(editor.FileName));
			ExpressionResult result = ef.FindExpression(editor.Document.Text, cursor);
			LoggingService.Debug("CC: Beginning to type a word, result=" + result + ", context=" + result.Context);
			ShowCompletion(result, editor, '\0');
			return true;
		}
		
		static bool TryDeclarationTypeInference(ITextEditor editor, IDocumentLine curLine)
		{
			string lineText = editor.Document.GetText(curLine.Offset, curLine.Length);
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new System.IO.StringReader(lineText));
			if (lexer.NextToken().Kind != Tokens.Dim)
				return false;
			if (lexer.NextToken().Kind != Tokens.Identifier)
				return false;
			if (lexer.NextToken().Kind != Tokens.As)
				return false;
			Token t1 = lexer.NextToken();
			if (t1.Kind != Tokens.QuestionMark)
				return false;
			Token t2 = lexer.NextToken();
			if (t2.Kind != Tokens.Assign)
				return false;
			string expr = lineText.Substring(t2.Location.Column);
			LoggingService.Debug("DeclarationTypeInference: >" + expr + "<");
			ResolveResult rr = ParserService.Resolve(new ExpressionResult(expr),
			                                         editor.Caret.Line,
			                                         t2.Location.Column, editor.FileName,
			                                         editor.Document.Text);
			if (rr != null && rr.ResolvedType != null) {
				ClassFinder context = new ClassFinder(ParserService.GetParseInformation(editor.FileName), editor.Caret.Line, t1.Location.Column);
				VBNetAmbience ambience = new VBNetAmbience();
				if (CodeGenerator.CanUseShortTypeName(rr.ResolvedType, context))
					ambience.ConversionFlags = ConversionFlags.None;
				else
					ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedTypeNames;
				string typeName = ambience.Convert(rr.ResolvedType);
				using (editor.Document.OpenUndoGroup()) {
					int offset = curLine.Offset + t1.Location.Column - 1;
					editor.Document.Remove(offset, 1);
					editor.Document.Insert(offset, typeName);
				}
				editor.Caret.Column += typeName.Length - 1;
				return true;
			}
			return false;
		}
	}
}
