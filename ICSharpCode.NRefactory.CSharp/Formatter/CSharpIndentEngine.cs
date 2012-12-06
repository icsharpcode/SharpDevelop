using System;
using ICSharpCode.NRefactory.Editor;
using System.Text;

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

		int offset;

		void Reset()
		{
			offset = 0;
			thisLineindent.Reset();
			indent.Reset();
			pc = '\0';
			IsLineStart = true;
			addContinuation = false;
			inside = Inside.Empty;
			nextBody = currentBody = Body.None;
		}

		public void UpdateToOffset (int toOffset)
		{
			if (toOffset < offset)
				Reset();
			for (int i = offset; i < toOffset; i++)
				Push(document.GetCharAt(i));
		}

		Inside inside = Inside.Empty;
		bool IsLineStart = true;

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

		[Flags]
		public enum Inside {
			Empty              = 0,
			
			PreProcessor       = (1 << 0),
			
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

		char pc;
		int parens = 0;
		void Push(char ch)
		{
			if (inside.HasFlag (Inside.VerbatimString) && pc == '"' && ch != '"') {
				inside &= ~Inside.String;
			}
			Console.WriteLine(ch);
			switch (ch) {
				case '#':
					if (IsLineStart)
						inside = Inside.PreProcessor;
					break;
				case '/':
					if (IsInStringOrChar)
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
					if (IsInStringOrChar || IsInComment)
						break;
					if (pc == '/')
						inside |= Inside.MultiLineComment;
					break;
				case '\n':
				case '\r':
					inside &= ~(Inside.Comment | Inside.String | Inside.CharLiteral | Inside.PreProcessor);
					CheckKeyword(wordBuf.ToString());
					wordBuf.Length = 0;
					if (addContinuation) {
						indent.Push (IndentType.Continuation);
					}
					thisLineindent = indent.Clone ();
					addContinuation = false;
					IsLineStart = true;
					break;
				case '"':
					if (IsInComment)
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
					if (IsInComment || IsInStringOrChar)
						break;
					parens++;
					indent.Push (IndentType.Block);
					break;
				case '>':
				case ']':
				case ')':
					if (IsInComment || IsInStringOrChar)
						break;
					parens--;
					indent.Pop ();
					break;
				case '{':
					if (IsInComment || IsInStringOrChar)
						break;
					currentBody = nextBody;
					if (indent.Count > 0 && indent.Peek() == IndentType.Continuation)
						indent.Pop();
					addContinuation = false;
					AddIndentation (currentBody);
					break;
				case '}':
					if (IsInComment || IsInStringOrChar)
						break;
					indent.Pop ();
					if (indent.Count > 0 && indent.Peek() == IndentType.Continuation)
						indent.Pop();
					break;
				case ';':
					if (indent.Count > 0 && indent.Peek() == IndentType.Continuation)
						indent.Pop();
					break;
				case '\'':
					if (IsInComment || inside.HasFlag (Inside.String))
						break;
					if (inside.HasFlag (Inside.CharLiteral)) {
						if (pc != '\\')
							inside &= ~Inside.CharLiteral;
					} else {
						inside &= Inside.CharLiteral;
					}
					break;
			}

			if (!IsInComment && !IsInStringOrChar) {
				if ((wordBuf.Length == 0 ? char.IsLetter(ch) : char.IsLetterOrDigit(ch)) || ch == '_') {
					wordBuf.Append(ch);
				} else {
					CheckKeyword(wordBuf.ToString());
					wordBuf.Length = 0;
				}
			}
			if (addContinuation) {
				indent.Push (IndentType.Continuation);
				addContinuation = false;
			}
			IsLineStart &= ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r';
			pc = ch;
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

		Body currentBody;
		Body nextBody;
		bool addContinuation;
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

