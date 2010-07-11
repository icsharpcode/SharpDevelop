// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
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
			if (IsInComment(editor))
				return CodeCompletionKeyPressResult.None;
			
			switch (ch) {
				case '\n':
					break;
				case ' ':
					if (CodeCompletionOptions.KeywordCompletionEnabled) {
						string word = editor.GetWordBeforeCaret();
						if (!string.IsNullOrEmpty(word)) {
							var list = HandleKeyword(editor, word);
							editor.ShowCompletionWindow(list);
							return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
				default:
					if (CodeCompletionOptions.CompleteWhenTyping) {
						if (editor.SelectionLength > 0) {
							// allow code completion when overwriting an identifier
							int endOffset = editor.SelectionStart + editor.SelectionLength;
							// but block code completion when overwriting only part of an identifier
							if (endOffset < editor.Document.TextLength && char.IsLetterOrDigit(editor.Document.GetCharAt(endOffset)))
								return CodeCompletionKeyPressResult.None;
							
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
								var provider = new VBNetCodeCompletionDataProvider(result);
								provider.ShowTemplates = true;
								provider.AllowCompleteExistingExpression = afterUnderscore;
								provider.ShowCompletion(editor);
								return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
							}
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
		}
		
		bool IsInComment(ITextEditor editor)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, editor.Document.CreateReader());
			
			Token t = lexer.NextToken();
			bool inXml = false;
			
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
				return true;
			} else {
				string lineText = editor.Document.GetLine(t.Location.Line).Text;
				
				bool inString = false;
				bool inComment = false;
				
				for (int i = 0; i < lineText.Length; i++) {
					char ch = lineText[i];
					
					if (!inComment && ch == '"')
						inString = !inString;
					if (!inString && ch == '\'')
						inComment = true;
				}
				
				return inComment;
			}
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
					var provider = new VBNetCodeCompletionDataProvider(result);
					provider.ShowTemplates = true;
					provider.AllowCompleteExistingExpression = afterUnderscore;
					provider.ShowCompletion(editor);
					return true;
				}
			}
			
			return false;
		}
		
		sealed class GlobalCompletionItemProvider : CodeCompletionItemProvider
		{
			public override ExpressionResult GetExpression(ITextEditor editor)
			{
				return new ExpressionResult("Global", ExpressionContext.Importable);
			}
		}
		
		ICompletionItemList HandleKeyword(ITextEditor editor, string word)
		{
			DefaultCompletionItemList list = new DefaultCompletionItemList();
			
			switch (word.ToLowerInvariant()) {
				case "option":
					return new TextCompletionItemProvider(
						"Explicit On", "Explicit Off",
						"Strict On", "Strict Off",
						"Compare Binary", "Compare Text",
						"Infer On", "Infer Off"
					).GenerateCompletionList(editor);
				case "imports":
					return new GlobalCompletionItemProvider().GenerateCompletionList(editor);
					break;
			}
			
			return list;
		}
	}
	

}
