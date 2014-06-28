// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Indentation.CSharp;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class CSharpFormattingStrategy : DefaultFormattingStrategy
	{
		#region Smart Indentation
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			int lineNr = line.LineNumber;
			DocumentAccessor acc = new DocumentAccessor(editor.Document, lineNr, lineNr);
			
			CSharpIndentationStrategy indentStrategy = new CSharpIndentationStrategy();
			indentStrategy.IndentationString = GetIndentationString(editor);
			indentStrategy.Indent(acc, false);
			
			string t = acc.Text;
			if (t.Length == 0) {
				// use AutoIndentation for new lines in comments / verbatim strings.
				base.IndentLine(editor, line);
			}
		}
		
		public override void IndentLines(ITextEditor editor, int beginLine, int endLine)
		{
			DocumentAccessor acc = new DocumentAccessor(editor.Document, beginLine, endLine);
			CSharpIndentationStrategy indentStrategy = new CSharpIndentationStrategy();
			indentStrategy.IndentationString = GetIndentationString(editor);
			indentStrategy.Indent(acc, true);
		}
		
		CSharpFormattingOptionsContainer GetOptionsContainerForEditor(ITextEditor editor)
		{
			var currentProject = SD.ProjectService.FindProjectContainingFile(editor.FileName);
			if (currentProject != null) {
				var persistence = CSharpFormattingPolicies.Instance.GetProjectOptions(currentProject);
				if (persistence != null) {
					return persistence.OptionsContainer;
				}
			}
			
			return null;
		}
		
		string GetIndentationString(ITextEditor editor)
		{
			// Get current indentation option values
			int indentationSize = editor.Options.IndentationSize;
			bool convertTabsToSpaces = editor.Options.ConvertTabsToSpaces;
			var container = GetOptionsContainerForEditor(editor);
			if (container != null) {
				int? effectiveIndentationSize = container.GetEffectiveIndentationSize();
				if (effectiveIndentationSize.HasValue)
					indentationSize = effectiveIndentationSize.Value;
				bool? effectiveConvertTabsToSpaces = container.GetEffectiveConvertTabsToSpaces();
				if (effectiveConvertTabsToSpaces.HasValue)
					convertTabsToSpaces = effectiveConvertTabsToSpaces.Value;
			}
			
			if (convertTabsToSpaces)
				return new string(' ', indentationSize);
			else
				return "\t";
		}
		
		/* NR indent engine (temporarily?) disabled as per #447
		static void IndentSingleLine(CacheIndentEngine engine, IDocument document, IDocumentLine line)
		{
		engine.Update(line.EndOffset);
		if (engine.NeedsReindent) {
		var indentation = TextUtilities.GetWhitespaceAfter(document, line.Offset);
		// replacing the indentation in two steps is necessary to make the caret move accordingly.
		document.Replace(indentation.Offset, indentation.Length, "");
		document.Replace(indentation.Offset, 0, engine.ThisLineIndent);
		engine.ResetEngineToPosition(line.Offset);
		}
		}
		
		static CacheIndentEngine CreateIndentEngine(IDocument document, TextEditorOptions options)
		{
		IProject currentProject = null;
		var projectService = SD.GetService<IProjectService>();
		if (projectService != null) {
		currentProject = projectService.FindProjectContainingFile(new FileName(document.FileName));
		}
		var formattingOptions = CSharpFormattingOptionsPersistence.GetProjectOptions(currentProject);
		var engine = new CSharpIndentEngine(document, options, formattingOptions.OptionsContainer.GetEffectiveOptions());
		return new CacheIndentEngine(engine);
		}
		*/
		#endregion
		
		#region Private functions
		bool NeedCurlyBracket(string text)
		{
			int curlyCounter = 0;
			
			bool inString = false;
			bool inChar   = false;
			bool verbatim = false;
			
			bool lineComment  = false;
			bool blockComment = false;
			
			for (int i = 0; i < text.Length; ++i) {
				switch (text[i]) {
					case '\r':
					case '\n':
						lineComment = false;
						inChar = false;
						if (!verbatim) inString = false;
						break;
					case '/':
						if (blockComment) {
							Debug.Assert(i > 0);
							if (text[i - 1] == '*') {
								blockComment = false;
							}
						}
						if (!inString && !inChar && i + 1 < text.Length) {
							if (!blockComment && text[i + 1] == '/') {
								lineComment = true;
							}
							if (!lineComment && text[i + 1] == '*') {
								blockComment = true;
							}
						}
						break;
					case '"':
						if (!(inChar || lineComment || blockComment)) {
							if (inString && verbatim) {
								if (i + 1 < text.Length && text[i + 1] == '"') {
									++i; // skip escaped quote
									inString = false; // let the string go on
								} else {
									verbatim = false;
								}
							} else if (!inString && i > 0 && text[i - 1] == '@') {
								verbatim = true;
							}
							inString = !inString;
						}
						break;
					case '\'':
						if (!(inString || lineComment || blockComment)) {
							inChar = !inChar;
						}
						break;
					case '{':
						if (!(inString || inChar || lineComment || blockComment)) {
							++curlyCounter;
						}
						break;
					case '}':
						if (!(inString || inChar || lineComment || blockComment)) {
							--curlyCounter;
						}
						break;
					case '\\':
						if ((inString && !verbatim) || inChar)
							++i; // skip next character
						break;
			}
			}
			return curlyCounter > 0;
		}
		
		
		bool IsInsideStringOrComment(ITextEditor textArea, IDocumentLine curLine, int cursorOffset)
		{
			// scan cur line if it is inside a string or single line comment (//)
			bool insideString  = false;
			char stringstart = ' ';
			bool verbatim = false; // true if the current string is verbatim (@-string)
			char c = ' ';
			char lastchar;
			
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				lastchar = c;
				c = textArea.Document.GetCharAt(i);
				if (insideString) {
					if (c == stringstart) {
						if (verbatim && i + 1 < cursorOffset && textArea.Document.GetCharAt(i + 1) == '"') {
							++i; // skip escaped character
						} else {
							insideString = false;
						}
					} else if (c == '\\' && !verbatim) {
						++i; // skip escaped character
					}
				} else if (c == '/' && i + 1 < cursorOffset && textArea.Document.GetCharAt(i + 1) == '/') {
					return true;
				} else if (c == '"' || c == '\'') {
					stringstart = c;
					insideString = true;
					verbatim = (c == '"') && (lastchar == '@');
				}
			}
			
			return insideString;
		}
		
		bool IsInsideDocumentationComment(ITextEditor textArea, IDocumentLine curLine, int cursorOffset)
		{
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = textArea.Document.GetCharAt(i);
				if (ch == '"') {
					// parsing strings correctly is too complicated (see above),
					// but I don't know any case where a doc comment is after a string...
					return false;
				}
				if (ch == '/' && i + 2 < cursorOffset && textArea.Document.GetCharAt(i + 1) == '/' && textArea.Document.GetCharAt(i + 2) == '/') {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Gets the next member after the specified caret position.
		/// </summary>
		IUnresolvedEntity GetMemberAfter(ITextEditor editor, int caretLine)
		{
			FileName fileName = editor.FileName;
			IUnresolvedEntity nextElement = null;
			if (fileName != null) {
				IUnresolvedFile unresolvedFile = SD.ParserService.ParseFile(fileName, editor.Document);
				if (unresolvedFile != null) {
					var currentClass = unresolvedFile.GetInnermostTypeDefinition(caretLine, 0);
					int nextElementLine = int.MaxValue;
					if (currentClass == null) {
						foreach (var c in unresolvedFile.TopLevelTypeDefinitions) {
							if (c.Region.BeginLine < nextElementLine && c.Region.BeginLine > caretLine) {
								nextElementLine = c.Region.BeginLine;
								nextElement = c;
							}
						}
					} else {
						foreach (var c in currentClass.NestedTypes) {
							if (c.Region.BeginLine < nextElementLine && c.Region.BeginLine > caretLine) {
								nextElementLine = c.Region.BeginLine;
								nextElement = c;
							}
						}
						foreach (var m in currentClass.Members) {
							if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
								nextElementLine = m.Region.BeginLine;
								nextElement = m;
							}
						}
					}
				}
			}
			return nextElement;
		}
		#endregion
		
		#region FormatLine
		
		bool NeedEndregion(IDocument document)
		{
			int regions = 0;
			int endregions = 0;
			for (int i = 1; i <= document.LineCount; i++) {
				string text = document.GetText(document.GetLineByNumber(i)).Trim();
				if (text.StartsWith("#region", StringComparison.Ordinal)) {
					++regions;
				} else if (text.StartsWith("#endregion", StringComparison.Ordinal)) {
					++endregions;
				}
			}
			return regions > endregions;
		}
		
		public override void FormatLines(ITextEditor textArea)
		{
			// Format current selection or whole document
			int formattedTextOffset = 0;
			int formattedTextLength = textArea.Document.TextLength;
			if (textArea.SelectionLength != 0) {
				formattedTextOffset = textArea.SelectionStart;
				formattedTextLength = textArea.SelectionLength;
			}
			FormatCode(textArea, formattedTextOffset, formattedTextLength, false);
		}
		
		/// <summary>
		/// Formats a code section according to currently effective formatting settings.
		/// </summary>
		/// <param name="textArea">Text editor instance to format code in.</param>
		/// <param name="offset">Start offset of formatted code.</param>
		/// <param name="length">Length of formatted code.</param>
		/// <param name="respectAutoFormattingSetting">
		/// Set to <c>true</c> to perform formatting only if auto-formatting setting is active.
		/// If <c>false</c>, formatting will be performed in any case.
		/// </param>
		/// <returns><c>True</c>, if code has been formatted, <c>false</c> if auto-formatting is currently forbidden.</returns>
		private bool FormatCode(ITextEditor textArea, int offset, int length, bool respectAutoFormattingSetting)
		{
			if ((offset > textArea.Document.TextLength) || ((offset + length) > textArea.Document.TextLength))
				return false;
			if (respectAutoFormattingSetting && !CSharpFormattingPolicies.AutoFormatting)
				return false;
			
			using (textArea.Document.OpenUndoGroup()) {
				var formattingOptions = CSharpFormattingPolicies.Instance.GetProjectOptions(SD.ProjectService.CurrentProject);
				try {
					CSharpFormatterHelper.Format(textArea, offset, length, formattingOptions.OptionsContainer);
				} catch (Exception) {
					// Exceptions in formatting might happen if code contains syntax errors, we have to catch them
					return false;
				}
				return true;
			}
		}
		
		public override void FormatLine(ITextEditor textArea, char ch) // used for comment tag formater/inserter
		{
			using (textArea.Document.OpenUndoGroup()) {
				FormatLineInternal(textArea, textArea.Caret.Line, textArea.Caret.Offset, ch);
			}
		}
		
		bool FormatStatement(ITextEditor textArea, int cursorOffset, int formattingStartOffset)
		{
			var line = textArea.Document.GetLineByOffset(formattingStartOffset);
			int lineOffset = line.Offset;
			// Walk up the lines until we arrive at previous statement, block, comment or preprocessor directive
			while (line.PreviousLine != null) {
				line = line.PreviousLine;
				string lineText = textArea.Document.GetText(line.Offset, line.Length);
				if (IsLineEndOfStatement(lineText)) {
					// Previous line is another statement, don't format it
					break;
				}
				lineOffset = line.Offset;
			}
			
			return FormatCode(textArea, lineOffset, cursorOffset - lineOffset, true);
		}
		
		void FormatLineInternal(ITextEditor textArea, int lineNr, int cursorOffset, char ch)
		{
			IDocumentLine curLine   = textArea.Document.GetLineByNumber(lineNr);
			IDocumentLine lineAbove = lineNr > 1 ? textArea.Document.GetLineByNumber(lineNr - 1) : null;
			string terminator = DocumentUtilities.GetLineTerminator(textArea.Document, lineNr);
			
			string curLineText;
			// local string for curLine segment
			if (ch == '/') {
				curLineText = textArea.Document.GetText(curLine);
				string lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
				if (curLineText != null && curLineText.EndsWith("///", StringComparison.Ordinal) && (lineAboveText == null || !lineAboveText.Trim().StartsWith("///", StringComparison.Ordinal))) {
					string indentation = DocumentUtilities.GetWhitespaceAfter(textArea.Document, curLine.Offset);
					IUnresolvedEntity member = GetMemberAfter(textArea, lineNr);
					if (member != null) {
						StringBuilder sb = new StringBuilder();
						sb.Append(" <summary>");
						sb.Append(terminator);
						sb.Append(indentation);
						sb.Append("/// ");
						sb.Append(terminator);
						sb.Append(indentation);
						sb.Append("/// </summary>");
						
						IUnresolvedMethod method = null;
						if (member is IUnresolvedMethod) {
							method = (IUnresolvedMethod)member;
						} else if (member is IUnresolvedTypeDefinition) {
							IUnresolvedTypeDefinition type = (IUnresolvedTypeDefinition) member;
							if (type.Kind == TypeKind.Delegate) {
								method = type.Methods.FirstOrDefault(m => m.Name == "Invoke");
							}
						}
						
						if (method != null) {
							for (int i = 0; i < method.Parameters.Count; ++i) {
								sb.Append(terminator);
								sb.Append(indentation);
								sb.Append("/// <param name=\"");
								sb.Append(method.Parameters[i].Name);
								sb.Append("\"></param>");
							}
							if (!method.IsConstructor) {
								KnownTypeReference returnType = method.ReturnType as KnownTypeReference;
								if (returnType == null || returnType.KnownTypeCode != KnownTypeCode.Void) {
									sb.Append(terminator);
									sb.Append(indentation);
									sb.Append("/// <returns></returns>");
								}
							}
						}
						
						textArea.Document.Insert(cursorOffset, sb.ToString());
						textArea.Caret.Offset = cursorOffset + indentation.Length + "/// ".Length + " <summary>".Length + terminator.Length;
					}
				}
				return;
			}
			
			if (ch != '\n' && ch != '>') {
				if (IsInsideStringOrComment(textArea, curLine, cursorOffset)) {
					return;
				}
			}
			switch (ch) {
				case '>':
					if (IsInsideDocumentationComment(textArea, curLine, cursorOffset)) {
						curLineText = textArea.Document.GetText(curLine);
						int column = cursorOffset - curLine.Offset;
						int index = Math.Min(column - 1, curLineText.Length - 1);
						
						while (index >= 0 && curLineText[index] != '<') {
							--index;
							if(curLineText[index] == '/')
								return; // the tag was an end tag or already
						}
						
						if (index > 0) {
							StringBuilder commentBuilder = new StringBuilder("");
							for (int i = index; i < curLineText.Length && i < column && !Char.IsWhiteSpace(curLineText[i]); ++i) {
								commentBuilder.Append(curLineText[ i]);
							}
							string tag = commentBuilder.ToString().Trim();
							if (!tag.EndsWith(">", StringComparison.Ordinal)) {
								tag += ">";
							}
							if (!tag.StartsWith("/", StringComparison.Ordinal)) {
								textArea.Document.Insert(cursorOffset, "</" + tag.Substring(1), AnchorMovementType.BeforeInsertion);
							}
						}
					}
					break;
				case ':':
				case ')':
				case ']':
				case '{':
					//if (textArea.Document.TextEditorProperties.IndentStyle == IndentStyle.Smart) {
					IndentLine(textArea, curLine);
					//}
					break;
				case '}':
					// Try to get corresponding block beginning brace
					var bracketSearchResult = textArea.Language.BracketSearcher.SearchBracket(textArea.Document, cursorOffset);
					if (bracketSearchResult != null) {
						// Format the block
						if (!FormatStatement(textArea, cursorOffset, bracketSearchResult.OpeningBracketOffset)) {
							// No auto-formatting seems to be active, at least indent the line
							IndentLine(textArea, curLine);
						}
					}
					break;
				case ';':
					// Format this line
					if (!FormatStatement(textArea, cursorOffset, cursorOffset)) {
						// No auto-formatting seems to be active, at least indent the line
						IndentLine(textArea, curLine);
					}
					break;
				case '\n':
					string lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
					// curLine might have some text which should be added to indentation
					curLineText = textArea.Document.GetText(curLine);
					
					if (lineAboveText != null && lineAboveText.Trim().StartsWith("#region", StringComparison.Ordinal)
						&& NeedEndregion(textArea.Document))
					{
						textArea.Document.Insert(cursorOffset, "#endregion");
						return;
					}
					
					IHighlighter highlighter = textArea.GetService(typeof(IHighlighter)) as IHighlighter;
					bool isInMultilineComment = false;
					bool isInMultilineString = false;
					if (highlighter != null && lineAbove != null) {
						var spanStack = highlighter.GetColorStack(lineNr).Select(c => c.Name).ToArray();
						isInMultilineComment = spanStack.Contains(HighlighterKnownSpanNames.Comment);
						isInMultilineString = spanStack.Contains(HighlighterKnownSpanNames.String);
					}
					bool isInNormalCode = !(isInMultilineComment || isInMultilineString);
					
					if (lineAbove != null && isInMultilineComment) {
						string lineAboveTextTrimmed = lineAboveText.TrimStart();
						if (lineAboveTextTrimmed.StartsWith("/*", StringComparison.Ordinal)) {
							textArea.Document.Insert(cursorOffset, " * ");
							return;
						}
						
						if (lineAboveTextTrimmed.StartsWith("*", StringComparison.Ordinal)) {
							textArea.Document.Insert(cursorOffset, "* ");
							return;
						}
					}
					
					if (lineAbove != null && isInNormalCode) {
						IDocumentLine nextLine  = lineNr + 1 <= textArea.Document.LineCount ? textArea.Document.GetLineByNumber(lineNr + 1) : null;
						string nextLineText = (nextLine != null) ? textArea.Document.GetText(nextLine) : "";
						
						int indexAbove = lineAboveText.IndexOf("///", StringComparison.Ordinal);
						int indexNext = nextLineText.IndexOf("///", StringComparison.Ordinal);
						if (indexAbove > 0 && (indexNext != -1 || indexAbove + 4 < lineAbove.Length)) {
							textArea.Document.Insert(cursorOffset, "/// ");
							return;
						}
						
						if (IsInNonVerbatimString(lineAboveText, curLineText)) {
							textArea.Document.Insert(cursorOffset, "\"");
							textArea.Document.Insert(lineAbove.Offset + lineAbove.Length,
								"\" +");
						}
					}
					if (textArea.Options.AutoInsertBlockEnd && lineAbove != null && isInNormalCode) {
						string oldLineText = textArea.Document.GetText(lineAbove);
						if (oldLineText.EndsWith("{", StringComparison.Ordinal)) {
							if (NeedCurlyBracket(textArea.Document.Text)) {
								int insertionPoint = curLine.Offset + curLine.Length;
								textArea.Document.Insert(insertionPoint, terminator + "}");
								IndentLine(textArea, textArea.Document.GetLineByNumber(lineNr + 1));
								textArea.Caret.Offset = insertionPoint;
							}
						}
					}
					return;
		}
		}
		
		bool IsLineEndOfStatement(string lineText)
		{
			string normalizedLine = null;
			
			// Look if there is a comment at the end of line
			int indexOfSingleLineComment = lineText.LastIndexOf("//");
			if (indexOfSingleLineComment > -1) {
				normalizedLine = lineText.Substring(0, indexOfSingleLineComment);
			} else {
				normalizedLine = lineText;
			}
			
			normalizedLine = normalizedLine.Trim(' ', '\t');
			
			if (normalizedLine.EndsWith("*/")) {
				int indexOfMultiLineCommentStart = normalizedLine.LastIndexOf("/*");
				if (indexOfMultiLineCommentStart > -1) {
					normalizedLine = normalizedLine.Substring(0, indexOfMultiLineCommentStart);
				} else {
					// Seems to be a multiline comment (no comment start on this line)
					return true;
				}
			}
			
			// Usual statement endings
			if (normalizedLine.StartsWith("#")
				|| normalizedLine.EndsWith(";")
				|| normalizedLine.EndsWith("{")
				|| normalizedLine.EndsWith("}"))
				return true;
			
			return false;
		}
		
		/// <summary>
		/// Checks if the cursor is inside a non-verbatim string.
		/// This method is used to check if a line break was inserted in a string.
		/// The text editor has already broken the line for us, so we just need to check
		/// the two lines.
		/// </summary>
		/// <param name="start">The part before the line break</param>
		/// <param name="end">The part after the line break</param>
		/// <returns>
		/// True, when the line break was inside a non-verbatim-string, so when
		/// start does not contain a comment, but a non-even number of ", and
		/// end contains a non-even number of " before the first comment.
		/// </returns>
		bool IsInNonVerbatimString(string start, string end)
		{
			bool inString = false;
			bool inChar = false;
			for (int i = 0; i < start.Length; ++i) {
				char c = start[i];
				if (c == '"' && !inChar) {
					if (!inString && i > 0 && start[i - 1] == '@')
						return false; // no string line break for verbatim strings
					inString = !inString;
				} else if (c == '\'' && !inString) {
					inChar = !inChar;
				}
				if (!inString && i > 0 && start[i - 1] == '/' && (c == '/' || c == '*'))
					return false;
				if (inString && start[i] == '\\')
					++i;
			}
			if (!inString) return false;
			// we are possibly in a string, or a multiline string has just ended here
			// check if the closing double quote is in end
			for (int i = 0; i < end.Length; ++i) {
				char c = end[i];
				if (c == '"' && !inChar) {
					if (!inString && i > 0 && end[i - 1] == '@')
						break; // no string line break for verbatim strings
					inString = !inString;
				} else if (c == '\'' && !inString) {
					inChar = !inChar;
				}
				if (!inString && i > 0 && end[i - 1] == '/' && (c == '/' || c == '*'))
					break;
				if (inString && end[i] == '\\')
					++i;
			}
			// return true if the string was closed properly
			return !inString;
		}
		#endregion
		
		#region SearchBracket helper functions
		static int ScanLineStart(IDocument document, int offset)
		{
			for (int i = offset - 1; i > 0; --i) {
				if (document.GetCharAt(i) == '\n')
					return i + 1;
			}
			return 0;
		}
		
		/// <summary>
		/// Gets the type of code at offset.<br/>
		/// 0 = Code,<br/>
		/// 1 = Comment,<br/>
		/// 2 = String<br/>
		/// Block comments and multiline strings are not supported.
		/// </summary>
		static int GetStartType(IDocument document, int linestart, int offset)
		{
			bool inString = false;
			bool inChar = false;
			bool verbatim = false;
			for(int i = linestart; i < offset; i++) {
				switch (document.GetCharAt(i)) {
					case '/':
						if (!inString && !inChar && i + 1 < document.TextLength) {
							if (document.GetCharAt(i + 1) == '/') {
								return 1;
							}
						}
						break;
					case '"':
						if (!inChar) {
							if (inString && verbatim) {
								if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"') {
									++i; // skip escaped quote
									inString = false; // let the string go on
								} else {
									verbatim = false;
								}
							} else if (!inString && i > 0 && document.GetCharAt(i - 1) == '@') {
								verbatim = true;
							}
							inString = !inString;
						}
						break;
					case '\'':
						if (!inString) inChar = !inChar;
						break;
					case '\\':
						if ((inString && !verbatim) || inChar)
							++i; // skip next character
						break;
			}
			}
			return (inString || inChar) ? 2 : 0;
		}
		#endregion
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithSingleLineComment(editor, "//");
		}
		/*
		#region SearchBracketBackward
		public override int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
		if (offset + 1 >= document.TextLength) return -1;
		// this method parses a c# document backwards to find the matching bracket
			
		// first try "quick find" - find the matching bracket if there is no string/comment in the way
		int quickResult = base.SearchBracketBackward(document, offset, openBracket, closingBracket);
		if (quickResult >= 0) return quickResult;
			
		// we need to parse the line from the beginning, so get the line start position
		int linestart = ScanLineStart(document, offset + 1);
			
		// we need to know where offset is - in a string/comment or in normal code?
		// ignore cases where offset is in a block comment
		int starttype = GetStartType(document, linestart, offset + 1);
		if (starttype != 0) {
		return -1; // start position is in a comment/string
		}
			
		// I don't see any possibility to parse a C# document backwards...
		// We have to do it forwards and push all bracket positions on a stack.
		Stack bracketStack = new Stack();
		bool  blockComment = false;
		bool  lineComment  = false;
		bool  inChar       = false;
		bool  inString     = false;
		bool  verbatim     = false;
			
		for(int i = 0; i <= offset; ++i) {
		char ch = document.GetCharAt(i);
		switch (ch) {
		case '\r':
		case '\n':
		lineComment = false;
		inChar = false;
		if (!verbatim) inString = false;
		break;
		case '/':
		if (blockComment) {
		Debug.Assert(i > 0);
		if (document.GetCharAt(i - 1) == '*') {
		blockComment = false;
		}
		}
		if (!inString && !inChar && i + 1 < document.TextLength) {
		if (!blockComment && document.GetCharAt(i + 1) == '/') {
		lineComment = true;
		}
		if (!lineComment && document.GetCharAt(i + 1) == '*') {
		blockComment = true;
		}
		}
		break;
		case '"':
		if (!(inChar || lineComment || blockComment)) {
		if (inString && verbatim) {
		if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"') {
		++i; // skip escaped quote
		inString = false; // let the string go
		} else {
		verbatim = false;
		}
		} else if (!inString && offset > 0 && document.GetCharAt(i - 1) == '@') {
		verbatim = true;
		}
		inString = !inString;
		}
		break;
		case '\'':
		if (!(inString || lineComment || blockComment)) {
		inChar = !inChar;
		}
		break;
		case '\\':
		if ((inString && !verbatim) || inChar)
		++i; // skip next character
		break;
		default :
		if (ch == openBracket) {
		if (!(inString || inChar || lineComment || blockComment)) {
		bracketStack.Push(i);
		}
		} else if (ch == closingBracket) {
		if (!(inString || inChar || lineComment || blockComment)) {
		if (bracketStack.Count > 0)
		bracketStack.Pop();
		}
		}
		break;
		}
		}
		if (bracketStack.Count > 0) return (int)bracketStack.Pop();
		return -1;
		}
		#endregion
		
		#region SearchBracketForward
		public override int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
		bool inString = false;
		bool inChar   = false;
		bool verbatim = false;
			
		bool lineComment  = false;
		bool blockComment = false;
			
		if (offset < 0) return -1;
			
		// first try "quick find" - find the matching bracket if there is no string/comment in the way
		int quickResult = base.SearchBracketForward(document, offset, openBracket, closingBracket);
		if (quickResult >= 0) return quickResult;
			
		// we need to parse the line from the beginning, so get the line start position
		int linestart = ScanLineStart(document, offset);
			
		// we need to know where offset is - in a string/comment or in normal code?
		// ignore cases where offset is in a block comment
		int starttype = GetStartType(document, linestart, offset);
		if (starttype != 0) return -1; // start position is in a comment/string
			
		int brackets = 1;
			
		while (offset < document.TextLength) {
		char ch = document.GetCharAt(offset);
		switch (ch) {
		case '\r':
		case '\n':
		lineComment = false;
		inChar = false;
		if (!verbatim) inString = false;
		break;
		case '/':
		if (blockComment) {
		Debug.Assert(offset > 0);
		if (document.GetCharAt(offset - 1) == '*') {
		blockComment = false;
		}
		}
		if (!inString && !inChar && offset + 1 < document.TextLength) {
		if (!blockComment && document.GetCharAt(offset + 1) == '/') {
		lineComment = true;
		}
		if (!lineComment && document.GetCharAt(offset + 1) == '*') {
		blockComment = true;
		}
		}
		break;
		case '"':
		if (!(inChar || lineComment || blockComment)) {
		if (inString && verbatim) {
		if (offset + 1 < document.TextLength && document.GetCharAt(offset + 1) == '"') {
		++offset; // skip escaped quote
		inString = false; // let the string go
		} else {
		verbatim = false;
		}
		} else if (!inString && offset > 0 && document.GetCharAt(offset - 1) == '@') {
		verbatim = true;
		}
		inString = !inString;
		}
		break;
		case '\'':
		if (!(inString || lineComment || blockComment)) {
		inChar = !inChar;
		}
		break;
		case '\\':
		if ((inString && !verbatim) || inChar)
		++offset; // skip next character
		break;
		default :
		if (ch == openBracket) {
		if (!(inString || inChar || lineComment || blockComment)) {
		++brackets;
		}
		} else if (ch == closingBracket) {
		if (!(inString || inChar || lineComment || blockComment)) {
		--brackets;
		if (brackets == 0) {
		return offset;
		}
		}
		}
		break;
		}
		++offset;
		}
		return -1;
		}
		#endregion
		*/
	}
}
