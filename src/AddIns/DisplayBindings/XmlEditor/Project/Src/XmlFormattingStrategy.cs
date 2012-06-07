// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// This class currently inserts the closing tags to typed openening tags
	/// and does smart indentation for xml files.
	/// </summary>
	public class XmlFormattingStrategy : DefaultFormattingStrategy
	{
		public override void FormatLine(ITextEditor editor, char charTyped)
		{
			editor.Document.StartUndoableAction();
			try {
				if (charTyped == '>') {
					StringBuilder stringBuilder = new StringBuilder();
					int offset = Math.Min(editor.Caret.Offset - 2, editor.Document.TextLength - 1);
					while (true) {
						if (offset < 0) {
							break;
						}
						char ch = editor.Document.GetCharAt(offset);
						if (ch == '<') {
							string reversedTag = stringBuilder.ToString().Trim();
							if (!reversedTag.StartsWith("/", StringComparison.Ordinal) && !reversedTag.EndsWith("/", StringComparison.Ordinal)) {
								bool validXml = true;
								try {
									XmlDocument doc = new XmlDocument();
									doc.LoadXml(editor.Document.Text);
								} catch (XmlException) {
									validXml = false;
								}
								// only insert the tag, if something is missing
								if (!validXml) {
									StringBuilder tag = new StringBuilder();
									for (int i = reversedTag.Length - 1; i >= 0 && !Char.IsWhiteSpace(reversedTag[i]); --i) {
										tag.Append(reversedTag[i]);
									}
									string tagString = tag.ToString();
									if (tagString.Length > 0 && !tagString.StartsWith("!", StringComparison.Ordinal) && !tagString.StartsWith("?", StringComparison.Ordinal)) {
										int caretOffset = editor.Caret.Offset;
										editor.Document.Insert(editor.Caret.Offset, "</" + tagString + ">");
										editor.Caret.Offset = caretOffset;
									}
								}
							}
							break;
						}
						stringBuilder.Append(ch);
						--offset;
					}
				}
			} catch (Exception e) { // Insanity check
				Debug.Assert(false, e.ToString());
			}
			if (charTyped == '\n') {
				IndentLine(editor, editor.Document.GetLine(editor.Caret.Line));
			}
			editor.Document.EndUndoableAction();
		}
		
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			editor.Document.StartUndoableAction();
			try {
				TryIndent(editor, line.LineNumber, line.LineNumber);
			} catch (XmlException ex) {
				LoggingService.Debug(ex.ToString());
			} finally {
				editor.Document.EndUndoableAction();
			}
		}
		
		/// <summary>
		/// This function sets the indentlevel in a range of lines.
		/// </summary>
		public override void IndentLines(ITextEditor editor, int begin, int end)
		{
			editor.Document.StartUndoableAction();
			try {
				TryIndent(editor, begin, end);
			} catch (XmlException ex) {
				LoggingService.Debug(ex.ToString());
			} finally {
				editor.Document.EndUndoableAction();
			}
		}
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithBlockComment(editor, "<!--", "-->");
		}
		
		static void TryIndent(ITextEditor editor, int begin, int end)
		{
			string currentIndentation = "";
			Stack<string> tagStack = new Stack<string>();
			IDocument document = editor.Document;
			
			string tab = editor.Options.IndentationString;
			int nextLine = begin; // in #dev coordinates
			bool wasEmptyElement = false;
			XmlNodeType lastType = XmlNodeType.XmlDeclaration;
			using (StringReader stringReader = new StringReader(document.Text)) {
				XmlTextReader r = new XmlTextReader(stringReader);
				r.XmlResolver = null; // prevent XmlTextReader from loading external DTDs
				while (r.Read()) {
					if (wasEmptyElement) {
						wasEmptyElement = false;
						if (tagStack.Count == 0)
							currentIndentation = "";
						else
							currentIndentation = tagStack.Pop();
					}
					if (r.NodeType == XmlNodeType.EndElement) {
						if (tagStack.Count == 0)
							currentIndentation = "";
						else
							currentIndentation = tagStack.Pop();
					}
					
					while (r.LineNumber >= nextLine) {
						if (nextLine > end) break;
						if (lastType == XmlNodeType.CDATA || lastType == XmlNodeType.Comment) {
							nextLine++;
							continue;
						}
						// set indentation of 'nextLine'
						IDocumentLine line = document.GetLine(nextLine);
						string lineText = line.Text;
						
						string newText;
						// special case: opening tag has closing bracket on extra line: remove one indentation level
						if (lineText.Trim() == ">")
							newText = tagStack.Peek() + lineText.Trim();
						else
							newText = currentIndentation + lineText.Trim();
						
						document.SmartReplaceLine(line, newText);
						nextLine++;
					}
					if (r.LineNumber > end)
						break;
					wasEmptyElement = r.NodeType == XmlNodeType.Element && r.IsEmptyElement;
					string attribIndent = null;
					if (r.NodeType == XmlNodeType.Element) {
						tagStack.Push(currentIndentation);
						if (r.LineNumber < begin)
							currentIndentation = DocumentUtilitites.GetWhitespaceAfter(editor.Document, editor.Document.PositionToOffset(r.LineNumber, 1));
						if (r.Name.Length < 16)
							attribIndent = currentIndentation + new string(' ', 2 + r.Name.Length);
						else
							attribIndent = currentIndentation + tab;
						currentIndentation += tab;
					}
					lastType = r.NodeType;
					if (r.NodeType == XmlNodeType.Element && r.HasAttributes) {
						int startLine = r.LineNumber;
						r.MoveToAttribute(0); // move to first attribute
						if (r.LineNumber != startLine)
							attribIndent = currentIndentation; // change to tab-indentation
						r.MoveToAttribute(r.AttributeCount - 1);
						while (r.LineNumber >= nextLine) {
							if (nextLine > end) break;
							// set indentation of 'nextLine'
							IDocumentLine line = document.GetLine(nextLine);
							string newText = attribIndent + line.Text.Trim();
							document.SmartReplaceLine(line, newText);
							nextLine++;
						}
					}
				}
				r.Close();
			}
		}
	}
}
