//
// CSharpIndentEngine.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using ICSharpCode.NRefactory.Editor;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace ICSharpCode.NRefactory.CSharp
{
	public class CSharpIndentEngine
	{
		readonly IDocument document;
		readonly CSharpFormattingOptions options;
		readonly TextEditorOptions textEditorOptions;
		readonly StringBuilder wordBuf = new StringBuilder();

		Indent thisLineindent;
		Indent indent;

		public IList<string> ConditionalSymbols {
			get;
			set;
		}

		public string ThisLineIndent {
			get {
				return thisLineindent.IndentString;
			}
		}

		public string NewLineIndent {
			get {
				return indent.IndentString;
			}
		}

		public CSharpIndentEngine(IDocument document, TextEditorOptions textEditorOptions, CSharpFormattingOptions formattingOptions)
		{
			this.document = document;
			this.options = formattingOptions;
			this.textEditorOptions = textEditorOptions;
			this.indent = new Indent(textEditorOptions);
			this.thisLineindent = new Indent(textEditorOptions);
		}

		CSharpIndentEngine (CSharpIndentEngine prototype)
		{
			this.document = prototype.document;
			this.options = prototype.options;
			this.textEditorOptions = prototype.textEditorOptions;
			this.indent = prototype.indent.Clone();
			this.thisLineindent = prototype.thisLineindent.Clone();
			this.offset = prototype.offset;
			this.inside = prototype.inside;
			this.IsLineStart = prototype.IsLineStart;
			this.pc = prototype.pc;
			this.parenStack = new Stack<TextLocation>(prototype.parenStack.Reverse ());
			this.currentBody = prototype.currentBody;
			this.nextBody = prototype.nextBody;
			this.addContinuation = prototype.addContinuation;
			this.line = prototype.line;
			this.col = prototype.col;
			this.popNextParenBlock = prototype.popNextParenBlock;
		}

		public CSharpIndentEngine Clone ()
		{
			return new CSharpIndentEngine(this);
		}

		int offset;
		Inside inside = Inside.Empty;
		bool IsLineStart = true;
		char pc;
		Stack<TextLocation> parenStack = new Stack<TextLocation> ();
		Body currentBody;
		Body nextBody;
		bool addContinuation;
		int line, col;
		bool popNextParenBlock;

		bool readPreprocessorExpression;


		void Reset()
		{
			offset = 0;
			thisLineindent.Reset();
			indent.Reset();
			pc = '\0';
			IsLineStart = true;
			addContinuation = false;
			popNextParenBlock = false;
			parenStack.Clear();
			inside = Inside.Empty;
			nextBody = currentBody = Body.None;
			line = col = 1;
		}

		public void UpdateToOffset (int toOffset)
		{
			if (toOffset < offset)
				Reset();
			for (int i = offset; i < toOffset; i++)
				Push(document.GetCharAt(i));
		}


		bool IsInStringOrChar {
			get {
				return inside.HasFlag (Inside.StringOrChar);
			}
		}

		bool IsInComment {
			get {
				return inside.HasFlag (Inside.Comment);
			}
		}

		bool IsInPreProcessorComment {
			get {
				return inside.HasFlag (Inside.PreProcessorComment);
			}
		}
		 
		[Flags]
		public enum Inside {
			Empty              = 0,
			
			PreProcessor       = (1 << 0),
			PreProcessorComment = (1 << 12),

			MultiLineComment   = (1 << 1),
			LineComment        = (1 << 2),
			DocComment         = (1 << 11),
			Comment            = (MultiLineComment | LineComment | DocComment),
			
			VerbatimString     = (1 << 3),
			StringLiteral      = (1 << 4),
			CharLiteral        = (1 << 5),
			String             = (VerbatimString | StringLiteral),
			StringOrChar       = (String | CharLiteral),
			
			Attribute          = (1 << 6),
			ParenList          = (1 << 7),
			
			FoldedStatement    = (1 << 8),
			Block              = (1 << 9),
			Case               = (1 << 10),
			
			FoldedOrBlock      = (FoldedStatement | Block),
			FoldedBlockOrCase  = (FoldedStatement | Block | Case)
		}

		#region Pre processor evaluation (from cs-tokenizer.cs)
		static bool is_identifier_start_character (int c)
		{
			return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_' || Char.IsLetter ((char)c);
		}

		static bool is_identifier_part_character (char c)
		{
			if (c >= 'a' && c <= 'z')
				return true;
			
			if (c >= 'A' && c <= 'Z')
				return true;
			
			if (c == '_' || (c >= '0' && c <= '9'))
				return true;
			
			if (c < 0x80)
				return false;
			
			return Char.IsLetter (c) || Char.GetUnicodeCategory (c) == UnicodeCategory.ConnectorPunctuation;
		}

		bool eval_val (string s)
		{
			if (s == "true")
				return true;
			if (s == "false")
				return false;
			
			return ConditionalSymbols != null && ConditionalSymbols.Contains (s);
		}
		
		bool pp_primary (ref string s)
		{
			s = s.Trim ();
			int len = s.Length;
			
			if (len > 0){
				char c = s [0];
				
				if (c == '('){
					s = s.Substring (1);
					bool val = pp_expr (ref s, false);
					if (s.Length > 0 && s [0] == ')'){
						s = s.Substring (1);
						return val;
					}
					return false;
				}
				
				if (is_identifier_start_character (c)){
					int j = 1;
					
					while (j < len){
						c = s [j];
						
						if (is_identifier_part_character (c)){
							j++;
							continue;
						}
						bool v = eval_val (s.Substring (0, j));
						s = s.Substring (j);
						return v;
					}
					bool vv = eval_val (s);
					s = "";
					return vv;
				}
			}
			return false;
		}
		
		bool pp_unary (ref string s)
		{
			s = s.Trim ();
			int len = s.Length;
			
			if (len > 0){
				if (s [0] == '!'){
					if (len > 1 && s [1] == '='){
						return false;
					}
					s = s.Substring (1);
					return ! pp_primary (ref s);
				} else
					return pp_primary (ref s);
			} else {
				return false;
			}
		}
		
		bool pp_eq (ref string s)
		{
			bool va = pp_unary (ref s);
			
			s = s.Trim ();
			int len = s.Length;
			if (len > 0){
				if (s [0] == '='){
					if (len > 2 && s [1] == '='){
						s = s.Substring (2);
						return va == pp_unary (ref s);
					} else {
						return false;
					}
				} else if (s [0] == '!' && len > 1 && s [1] == '='){
					s = s.Substring (2);
					
					return va != pp_unary (ref s);
					
				} 
			}
			
			return va;
			
		}
		
		bool pp_and (ref string s)
		{
			bool va = pp_eq (ref s);
			
			s = s.Trim ();
			int len = s.Length;
			if (len > 0){
				if (s [0] == '&'){
					if (len > 2 && s [1] == '&'){
						s = s.Substring (2);
						return (va & pp_and (ref s));
					} else {
						return false;
					}
				} 
			}
			return va;
		}
		
		//
		// Evaluates an expression for `#if' or `#elif'
		//
		bool pp_expr (ref string s, bool isTerm)
		{
			bool va = pp_and (ref s);
			s = s.Trim ();
			int len = s.Length;
			if (len > 0){
				char c = s [0];
				
				if (c == '|'){
					if (len > 2 && s [1] == '|'){
						s = s.Substring (2);
						return va | pp_expr (ref s, isTerm);
					} else {

						return false;
					}
				}
				if (isTerm) {
					return false;
				}
			}
			
			return va;
		}
		
		bool eval (string s)
		{
			bool v = pp_expr (ref s, true);
			s = s.Trim ();
			if (s.Length != 0){
				return false;
			}
			
			return v;
		}
		#endregion
		void Push(char ch)
		{
			if (readPreprocessorExpression) {
				wordBuf.Append(ch);
			}

			if (inside.HasFlag (Inside.VerbatimString) && pc == '"' && ch != '"') {
				inside &= ~Inside.String;
			}
			switch (ch) {
				case '#':
					if (IsLineStart)
						inside = Inside.PreProcessor;
					break;
				case '/':
					if (IsInStringOrChar || IsInPreProcessorComment)
						break;
					if (pc == '/') {
						if (inside.HasFlag (Inside.Comment)) {
							inside |= Inside.DocComment;
						} else {
							inside |= Inside.Comment;
						}
					}
					break;
				case '*':
					if (IsInStringOrChar || IsInComment || IsInPreProcessorComment)
						break;
					if (pc == '/')
						inside |= Inside.MultiLineComment;
					break;
				case '\t':
					var nextTabStop = (col - 1 + textEditorOptions.IndentSize) / textEditorOptions.IndentSize;
					col = 1 + nextTabStop * textEditorOptions.IndentSize;
					return;
				case '\r':
					
					if (readPreprocessorExpression) {
						if (!eval (wordBuf.ToString ()))
							inside |= Inside.PreProcessorComment;
					}

					inside &= ~(Inside.Comment | Inside.String | Inside.CharLiteral | Inside.PreProcessor);
					CheckKeyword(wordBuf.ToString());
					wordBuf.Length = 0;
					if (addContinuation) {
						indent.Push (IndentType.Continuation);
					}
					thisLineindent = indent.Clone ();
					addContinuation = false;
					IsLineStart = true;
					readPreprocessorExpression = false;
					col = 1;
					line++;
					break;
				case '\n':
					if (pc == '\r')
						break;
					goto case '\r';
				case '"':
					if (IsInComment || IsInPreProcessorComment)
						break;
					if (inside.HasFlag (Inside.String)) {
						if (pc != '\\')
							inside &= ~Inside.String;
						break;
					}

					if (pc =='@') {
						inside |= Inside.VerbatimString;
					} else {
						inside |= Inside.String;
					}
					break;
				case '<':
				case '[':
				case '(':
					if (IsInComment || IsInStringOrChar || IsInPreProcessorComment)
						break;
					parenStack.Push (new TextLocation (line, col));
					popNextParenBlock = true;
					indent.Push (IndentType.Block);
					break;
				case '>':
				case ']':
				case ')':
					if (IsInComment || IsInStringOrChar || IsInPreProcessorComment)
						break;
					if (popNextParenBlock)
						parenStack.Pop ();
					indent.Pop ();
					indent.ExtraSpaces = 0;
					break;
				case ',':
					if (IsInComment || IsInStringOrChar || IsInPreProcessorComment)
						break;
					if (parenStack.Count > 0 && parenStack.Peek ().Line == line) {
						indent.Pop ();
						popNextParenBlock = false;
						indent.ExtraSpaces = parenStack.Peek ().Column - 1 - thisLineindent.CurIndent;
					}
					break;
				case '{':
					if (IsInComment || IsInStringOrChar || IsInPreProcessorComment)
						break;
					currentBody = nextBody;
					if (indent.Count > 0 && indent.Peek() == IndentType.Continuation)
						indent.Pop();
					addContinuation = false;
					AddIndentation (currentBody);
					break;
				case '}':
					if (IsInComment || IsInStringOrChar || IsInPreProcessorComment)
						break;
					indent.Pop ();
					if (indent.Count > 0 && indent.Peek() == IndentType.Continuation)
						indent.Pop();
					break;
				case ';':
					if (IsInComment || IsInStringOrChar || IsInPreProcessorComment)
						break;
					if (indent.Count > 0 && indent.Peek() == IndentType.Continuation)
						indent.Pop();
					break;
				case '\'':
					if (IsInComment || inside.HasFlag (Inside.String) || IsInPreProcessorComment)
						break;
					if (inside.HasFlag (Inside.CharLiteral)) {
						if (pc != '\\')
							inside &= ~Inside.CharLiteral;
					} else {
						inside &= Inside.CharLiteral;
					}
					break;
			}

			if (!IsInComment && !IsInStringOrChar && !readPreprocessorExpression) {
				if ((wordBuf.Length == 0 ? char.IsLetter(ch) : char.IsLetterOrDigit(ch)) || ch == '_') {
					wordBuf.Append(ch);
				} else {
					if (inside.HasFlag (Inside.PreProcessor)) {
						if (wordBuf.ToString () == "endif") {
							inside &= ~Inside.PreProcessorComment;
						} else if (wordBuf.ToString () == "if") {
							readPreprocessorExpression = true;
						} else if (wordBuf.ToString () == "elif") {
							inside &= ~Inside.PreProcessorComment;
							readPreprocessorExpression = true;
						}
					} else {
						CheckKeyword(wordBuf.ToString());
					}
					wordBuf.Length = 0;
				}
			}
			if (addContinuation) {
				indent.Push (IndentType.Continuation);
				addContinuation = false;
			}
			IsLineStart &= ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
			pc = ch;
			if (ch != '\n' && ch != '\r')
				col++;
		}

		void AddIndentation(BraceStyle braceStyle)
		{
			switch (braceStyle) {
				case BraceStyle.DoNotChange:
				case BraceStyle.EndOfLine:
				case BraceStyle.EndOfLineWithoutSpace:
				case BraceStyle.NextLine:
				case BraceStyle.NextLineShifted:
				case BraceStyle.BannerStyle:
					indent.Push (IndentType.Block);
					break;
				
				case BraceStyle.NextLineShifted2:
					indent.Push (IndentType.DoubleBlock);
					break;
			}
		}

		void AddIndentation(Body body)
		{
			switch (body) {
				case Body.None:
					indent.Push (IndentType.Block);
					break;
				case Body.Namespace:
					AddIndentation (options.NamespaceBraceStyle);
					break;
				case Body.Class:
					AddIndentation (options.ClassBraceStyle);
					break;
				case Body.Struct:
					AddIndentation (options.StructBraceStyle);
					break;
				case Body.Interface:
					AddIndentation (options.InterfaceBraceStyle);
					break;
				case Body.Enum:
					AddIndentation (options.EnumBraceStyle);
					break;
				case Body.Switch:
					if (options.IndentSwitchBody)
						indent.Push (IndentType.Empty);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		enum Body
		{
			None,
			Namespace,
			Class, Struct, Interface, Enum,
			Switch
		}

		void CheckKeyword (string keyword)
		{
			switch (currentBody) {
				case Body.None:
					if (keyword == "namespace") {
						nextBody = Body.Namespace;
						return;
					}
					goto case Body.Namespace;
				case Body.Namespace:
					if (keyword == "class") {
						nextBody = Body.Class;
						return;
					}
					if (keyword == "enum") {
						nextBody = Body.Enum;
						return;
					}
					if (keyword == "struct") {
						nextBody = Body.Struct;
						return;
					}
					if (keyword == "interface") {
						nextBody = Body.Interface;
						return;
					}
					break;
				case Body.Class:
				case Body.Enum:
				case Body.Struct:
				case Body.Interface:
					if (keyword == "switch")
						nextBody = Body.Switch;
					if (keyword == "do" || keyword == "if" || keyword == "for" || keyword == "foreach" || keyword == "while") {
						addContinuation = true;
					}
					break;
			}
		}
	}
}

