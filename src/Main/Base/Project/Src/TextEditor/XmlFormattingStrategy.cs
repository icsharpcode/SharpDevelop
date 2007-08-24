// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.DefaultEditor
{
	/// <summary>
	/// This class currently inserts the closing tags to typed openening tags
	/// and does smart indentation for xml files.
	/// </summary>
	public class XmlFormattingStrategy : DefaultFormattingStrategy
	{
		public override void FormatLine(TextArea textArea, int lineNr, int caretOffset, char charTyped) // used for comment tag formater/inserter
		{
			textArea.Document.UndoStack.StartUndoGroup();
			try {
				if (charTyped == '>') {
					StringBuilder stringBuilder = new StringBuilder();
					int offset = Math.Min(caretOffset - 2, textArea.Document.TextLength - 1);
					while (true) {
						if (offset < 0) {
							break;
						}
						char ch = textArea.Document.GetCharAt(offset);
						if (ch == '<') {
							string reversedTag = stringBuilder.ToString().Trim();
							if (!reversedTag.StartsWith("/") && !reversedTag.EndsWith("/")) {
								bool validXml = true;
								try {
									XmlDocument doc = new XmlDocument();
									doc.LoadXml(textArea.Document.TextContent);
								} catch (Exception) {
									validXml = false;
								}
								// only insert the tag, if something is missing
								if (!validXml) {
									StringBuilder tag = new StringBuilder();
									for (int i = reversedTag.Length - 1; i >= 0 && !Char.IsWhiteSpace(reversedTag[i]); --i) {
										tag.Append(reversedTag[i]);
									}
									string tagString = tag.ToString();
									if (tagString.Length > 0 && !tagString.StartsWith("!") && !tagString.StartsWith("?")) {
										textArea.Document.Insert(caretOffset, "</" + tagString + ">");
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
				textArea.Caret.Column = IndentLine(textArea, lineNr);
			}
			textArea.Document.UndoStack.EndUndoGroup();
		}
		
		/// <summary>
		/// Define XML specific smart indenting for a line :)
		/// </summary>
		protected override int SmartIndentLine(TextArea textArea, int lineNr)
		{
			if (lineNr <= 0) return AutoIndentLine(textArea, lineNr);
			try {
				TryIndent(textArea, lineNr, lineNr);
				return GetIndentation(textArea, lineNr).Length;
			} catch (XmlException) {
				return AutoIndentLine(textArea, lineNr);
			}
		}
		
		/// <summary>
		/// This function sets the indentlevel in a range of lines.
		/// </summary>
		public override void IndentLines(TextArea textArea, int begin, int end)
		{
			textArea.Document.UndoStack.StartUndoGroup();
			try {
				TryIndent(textArea, begin, end);
			} catch (XmlException ex) {
				LoggingService.Debug(ex.ToString());
			} finally {
				textArea.Document.UndoStack.EndUndoGroup();
			}
		}
		
		#region Smart Indentation
		private void TryIndent(TextArea textArea, int begin, int end)
		{
			string currentIndentation = "";
			Stack tagStack = new Stack();
			IDocument document = textArea.Document;
			string tab = Tab.GetIndentationString(document);
			int nextLine = begin; // in #dev coordinates
			bool wasEmptyElement = false;
			XmlNodeType lastType = XmlNodeType.XmlDeclaration;
			// TextReader line number begin with 1, #dev line numbers with 0
			using (StringReader stringReader = new StringReader(document.TextContent)) {
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
					
					while (r.LineNumber > nextLine) { // caution: here we compare 1-based and 0-based line numbers
						if (nextLine > end) break;
						if (lastType == XmlNodeType.CDATA || lastType == XmlNodeType.Comment) {
							nextLine += 1;
							continue;
						}
						// set indentation of 'nextLine'
						LineSegment line = document.GetLineSegment(nextLine);
						string lineText = document.GetText(line);
						
						string newText;
						// special case: opening tag has closing bracket on extra line: remove one indentation level
						if (lineText.Trim() == ">")
							newText = (string)tagStack.Peek() + lineText.Trim();
						else
							newText = currentIndentation + lineText.Trim();
						
						if (newText != lineText) {
							document.Replace(line.Offset, line.Length, newText);
						}
						nextLine += 1;
					}
					if (r.LineNumber > end)
						break;
					wasEmptyElement = r.NodeType == XmlNodeType.Element && r.IsEmptyElement;
					string attribIndent = null;
					if (r.NodeType == XmlNodeType.Element) {
						tagStack.Push(currentIndentation);
						if (r.LineNumber < begin)
							currentIndentation = GetIndentation(textArea, r.LineNumber - 1);
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
						while (r.LineNumber > nextLine) { // caution: here we compare 1-based and 0-based line numbers
							if (nextLine > end) break;
							// set indentation of 'nextLine'
							LineSegment line = document.GetLineSegment(nextLine);
							string lineText = document.GetText(line);
							string newText = attribIndent + lineText.Trim();
							if (newText != lineText) {
								document.Replace(line.Offset, line.Length, newText);
							}
							nextLine += 1;
						}
					}
				}
				r.Close();
			}
		}
		#endregion
	}
}
