// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace CSharpBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class CSharpFormattingStrategy : DefaultFormattingStrategy
	{
		public CSharpFormattingStrategy()
		{
		}
		
		#region SmartIndentLine
		/// <summary>
		/// Define CSharp specific smart indenting for a line :)
		/// </summary>
		protected override int SmartIndentLine(TextArea textArea, int lineNr)
		{
			if (lineNr <= 0) {
				return AutoIndentLine(textArea, lineNr);
			}
			
			string oldText = textArea.Document.GetText(textArea.Document.GetLineSegment(lineNr));
			
			DocumentAccessor acc = new DocumentAccessor(textArea.Document, lineNr, lineNr);
			
			IndentationSettings set = new IndentationSettings();
			set.IndentString = Tab.GetIndentationString(textArea.Document);
			set.LeaveEmptyLines = false;
			IndentationReformatter r = new IndentationReformatter();
			
			r.Reformat(acc, set);
			
			string t = acc.Text;
			if (t.Length == 0) {
				// use AutoIndentation for new lines in comments / verbatim strings.
				return AutoIndentLine(textArea, lineNr);
			} else {
				int newIndentLength = t.Length - t.TrimStart().Length;
				int oldIndentLength = oldText.Length - oldText.TrimStart().Length;
				if (oldIndentLength != newIndentLength && lineNr == textArea.Caret.Position.Y) {
					// fix cursor position if indentation was changed
					int newX = textArea.Caret.Position.X - oldIndentLength + newIndentLength;
					textArea.Caret.Position = new TextLocation(Math.Max(newX, 0), lineNr);
				}
				return newIndentLength;
			}
		}
		
		/// <summary>
		/// This function sets the indentlevel in a range of lines.
		/// </summary>
		public override void IndentLines(TextArea textArea, int begin, int end)
		{
			if (textArea.Document.TextEditorProperties.IndentStyle != IndentStyle.Smart) {
				base.IndentLines(textArea, begin, end);
				return;
			}
			int cursorPos = textArea.Caret.Position.Y;
			int oldIndentLength = 0;
			
			if (cursorPos >= begin && cursorPos <= end)
				oldIndentLength = GetIndentation(textArea, cursorPos).Length;
			
			IndentationSettings set = new IndentationSettings();
			set.IndentString = Tab.GetIndentationString(textArea.Document);
			IndentationReformatter r = new IndentationReformatter();
			DocumentAccessor acc = new DocumentAccessor(textArea.Document, begin, end);
			r.Reformat(acc, set);
			
			if (cursorPos >= begin && cursorPos <= end) {
				int newIndentLength = GetIndentation(textArea, cursorPos).Length;
				if (oldIndentLength != newIndentLength) {
					// fix cursor position if indentation was changed
					int newX = textArea.Caret.Position.X - oldIndentLength + newIndentLength;
					textArea.Caret.Position = new TextLocation(Math.Max(newX, 0), cursorPos);
				}
			}
		}
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
		
		
		bool IsInsideStringOrComment(TextArea textArea, LineSegment curLine, int cursorOffset)
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
		
		bool IsInsideDocumentationComment(TextArea textArea, LineSegment curLine, int cursorOffset)
		{
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = textArea.Document.GetCharAt(i);
				if (ch == '"') {
					// parsing strings correctly is too complicated (see above),
					// but I don't now any case where a doc comment is after a string...
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
		object GetMemberAfter(TextArea textArea, int caretLine)
		{
			string fileName = textArea.MotherTextEditorControl.FileName;
			object nextElement = null;
			if (fileName != null && fileName.Length > 0 ) {
				ParseInformation parseInfo = ParserService.ParseFile(fileName, textArea.Document.TextContent);
				if (parseInfo != null) {
					ICompilationUnit currentCompilationUnit = parseInfo.BestCompilationUnit;
					if (currentCompilationUnit != null) {
						IClass currentClass = currentCompilationUnit.GetInnermostClass(caretLine, 0);
						int nextElementLine = int.MaxValue;
						if (currentClass == null) {
							foreach (IClass c in currentCompilationUnit.Classes) {
								if (c.Region.BeginLine < nextElementLine && c.Region.BeginLine > caretLine) {
									nextElementLine = c.Region.BeginLine;
									nextElement = c;
								}
							}
						} else {
							foreach (IClass c in currentClass.InnerClasses) {
								if (c.Region.BeginLine < nextElementLine && c.Region.BeginLine > caretLine) {
									nextElementLine = c.Region.BeginLine;
									nextElement = c;
								}
							}
							foreach (IMember m in currentClass.Methods) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
							foreach (IMember m in currentClass.Properties) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
							foreach (IMember m in currentClass.Fields) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
							foreach (IMember m in currentClass.Events) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
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
			foreach (LineSegment line in document.LineSegmentCollection) {
				string text = document.GetText(line).Trim();
				if (text.StartsWith("#region")) {
					++regions;
				} else if (text.StartsWith("#endregion")) {
					++endregions;
				}
			}
			return regions > endregions;
		}
		public override void FormatLine(TextArea textArea, int lineNr, int cursorOffset, char ch) // used for comment tag formater/inserter
		{
			textArea.Document.UndoStack.StartUndoGroup();
			FormatLineInternal(textArea, lineNr, cursorOffset, ch);
			textArea.Document.UndoStack.EndUndoGroup();
		}
		
		void FormatLineInternal(TextArea textArea, int lineNr, int cursorOffset, char ch)
		{
			LineSegment curLine   = textArea.Document.GetLineSegment(lineNr);
			LineSegment lineAbove = lineNr > 0 ? textArea.Document.GetLineSegment(lineNr - 1) : null;
			string terminator = textArea.TextEditorProperties.LineTerminator;
			
			//// local string for curLine segment
			string curLineText="";
			if (ch == '/') {
				curLineText   = textArea.Document.GetText(curLine);
				string lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
				if (curLineText != null && curLineText.EndsWith("///") && (lineAboveText == null || !lineAboveText.Trim().StartsWith("///"))) {
					string indentation = base.GetIndentation(textArea, lineNr);
					object member = GetMemberAfter(textArea, lineNr);
					if (member != null) {
						StringBuilder sb = new StringBuilder();
						sb.Append(" <summary>");
						sb.Append(terminator);
						sb.Append(indentation);
						sb.Append("/// ");
						sb.Append(terminator);
						sb.Append(indentation);
						sb.Append("/// </summary>");
						
						if (member is IMethod) {
							IMethod method = (IMethod)member;
							if (method.Parameters != null && method.Parameters.Count > 0) {
								for (int i = 0; i < method.Parameters.Count; ++i) {
									sb.Append(terminator);
									sb.Append(indentation);
									sb.Append("/// <param name=\"");
									sb.Append(method.Parameters[i].Name);
									sb.Append("\"></param>");
								}
							}
							if (method.ReturnType != null && !method.IsConstructor && method.ReturnType.FullyQualifiedName != "System.Void") {
								sb.Append(terminator);
								sb.Append(indentation);
								sb.Append("/// <returns></returns>");
							}
						}
						textArea.Document.Insert(cursorOffset, sb.ToString());
						
						textArea.Refresh();
						textArea.Caret.Position = textArea.Document.OffsetToPosition(cursorOffset + indentation.Length + "/// ".Length + " <summary>".Length + terminator.Length);
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
						curLineText  = textArea.Document.GetText(curLine);
						int column = textArea.Caret.Offset - curLine.Offset;
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
							if (!tag.EndsWith(">")) {
								tag += ">";
							}
							if (!tag.StartsWith("/")) {
								textArea.Document.Insert(textArea.Caret.Offset, "</" + tag.Substring(1));
							}
						}
					}
					break;
				case ':':
				case ')':
				case ']':
				case '}':
				case '{':
					if (textArea.Document.TextEditorProperties.IndentStyle == IndentStyle.Smart) {
						textArea.Document.FormattingStrategy.IndentLine(textArea, lineNr);
					}
					break;
				case '\n':
					string  lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
					//// curLine might have some text which should be added to indentation
					curLineText = "";
					if (curLine.Length > 0) {
						curLineText = textArea.Document.GetText(curLine);
					}
					
					LineSegment nextLine      = lineNr + 1 < textArea.Document.TotalNumberOfLines ? textArea.Document.GetLineSegment(lineNr + 1) : null;
					string      nextLineText  = lineNr + 1 < textArea.Document.TotalNumberOfLines ? textArea.Document.GetText(nextLine) : "";
					
					int addCursorOffset = 0;
					
					if (lineAboveText.Trim().StartsWith("#region") && NeedEndregion(textArea.Document)) {
						textArea.Document.Insert(curLine.Offset, "#endregion");
						textArea.Caret.Column = IndentLine(textArea, lineNr);
						return;
					}
					
					if (lineAbove.HighlightSpanStack != null && !lineAbove.HighlightSpanStack.IsEmpty) {
						if (!lineAbove.HighlightSpanStack.Peek().StopEOL) {	// case for /* style comments
							int index = lineAboveText.IndexOf("/*");
							if (index > 0) {
								StringBuilder indentation = new StringBuilder(GetIndentation(textArea, lineNr - 1));
								for (int i = indentation.Length; i < index; ++ i) {
									indentation.Append(' ');
								}
								//// adding curline text
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation.ToString() + " * " + curLineText);
								textArea.Caret.Column = indentation.Length + 3 + curLineText.Length;
								return;
							}
							
							index = lineAboveText.IndexOf("*");
							if (index > 0) {
								StringBuilder indentation = new StringBuilder(GetIndentation(textArea, lineNr - 1));
								for (int i = indentation.Length; i < index; ++ i) {
									indentation.Append(' ');
								}
								//// adding curline if present
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation.ToString() + "* " + curLineText);
								textArea.Caret.Column = indentation.Length + 2 + curLineText.Length;
								return;
							}
						} else { // don't handle // lines, because they're only one lined comments
							int indexAbove = lineAboveText.IndexOf("///");
							int indexNext  = nextLineText.IndexOf("///");
							if (indexAbove > 0 && (indexNext != -1 || indexAbove + 4 < lineAbove.Length)) {
								StringBuilder indentation = new StringBuilder(GetIndentation(textArea, lineNr - 1));
								for (int i = indentation.Length; i < indexAbove; ++ i) {
									indentation.Append(' ');
								}
								//// adding curline text if present
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation.ToString() + "/// " + curLineText);
								textArea.Caret.Column = indentation.Length + 4;
								return;
							}
							
							if (IsInNonVerbatimString(lineAboveText, curLineText)) {
								textArea.Document.Insert(lineAbove.Offset + lineAbove.Length,
								                         "\" +");
								curLine = textArea.Document.GetLineSegment(lineNr);
								textArea.Document.Insert(curLine.Offset, "\"");
								addCursorOffset = 1;
							}
						}
					}
					int result = IndentLine(textArea, lineNr) + addCursorOffset;
					if (textArea.TextEditorProperties.AutoInsertCurlyBracket) {
						string oldLineText = TextUtilities.GetLineAsString(textArea.Document, lineNr - 1);
						if (oldLineText.EndsWith("{")) {
							if (NeedCurlyBracket(textArea.Document.TextContent)) {
								textArea.Document.Insert(curLine.Offset + curLine.Length, terminator + "}");
								IndentLine(textArea, lineNr + 1);
							}
						}
					}
					textArea.Caret.Column = result;
					return;
			}
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
	}
}
