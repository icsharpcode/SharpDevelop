// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.Core;

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
				IndentLine(editor, editor.Document.GetLineForOffset(editor.Caret.Offset));
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
		#region Smart Indentation
		static void TryIndent(ITextEditor editor, int begin, int end)
		{
			string currentIndentation = "";
			Stack tagStack = new Stack();
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
							currentIndentation = (string)tagStack.Pop();
					}
					if (r.NodeType == XmlNodeType.EndElement) {
						if (tagStack.Count == 0)
							currentIndentation = "";
						else
							currentIndentation = (string)tagStack.Pop();
					}
					
					while (r.LineNumber >= nextLine) { // caution: here we compare 1-based and 0-based line numbers
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
							newText = (string)tagStack.Peek() + lineText.Trim();
						else
							newText = currentIndentation + lineText.Trim();
						
						if (newText != lineText) {
							document.Replace(line.Offset, line.Length, newText);
						}
						nextLine++;
					}
					if (r.LineNumber > end)
						break;
					wasEmptyElement = r.NodeType == XmlNodeType.Element && r.IsEmptyElement;
					string attribIndent = null;
					if (r.NodeType == XmlNodeType.Element) {
						tagStack.Push(currentIndentation);
						if (r.LineNumber < begin)
							currentIndentation = DocumentUtilitites.GetIndentation(editor.Document, r.LineNumber - 1);
						if (r.Name.Length < 16)
							attribIndent = currentIndentation + new String(' ', 2 + r.Name.Length);
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
							string lineText = line.Text;
							string newText = attribIndent + lineText.Trim();
							if (newText != lineText) {
								document.Replace(line.Offset, line.Length, newText);
							}
							nextLine++;
						}
					}
				}
				r.Close();
			}
		}
		#endregion
	}
}
