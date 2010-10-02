// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.Scripting
{
	public class LineIndenter
	{
		ITextEditor editor;
		IDocument document;
		IDocumentLine line;
		IDocumentLine previousLine;
		string previousLineText;
		
		public LineIndenter(ITextEditor editor, IDocumentLine line)
		{
			this.editor = editor;
			this.line = line;
			this.document = editor.Document;
		}
		
		public bool Indent()
		{
			if (IsFirstLine()) {
				return false;
			}

			GetPreviousLine();
			
			if (ShouldIncreaseLineIndent()) {
				IncreaseLineIndent();
				return true;
			} else if (ShouldDecreaseLineIndent()) {
				DecreaseLineIndent();
				return true;
			}
			return false;
		}
		
		bool IsFirstLine()
		{
			return line.LineNumber == 1;
		}
		
		void GetPreviousLine()
		{
			int lineNumber = line.LineNumber - 1;
			previousLine = document.GetLine(lineNumber);
			previousLineText = previousLine.Text.Trim();
		}
		
		protected string PreviousLine {
			get { return previousLineText; }
		}
		
		protected virtual bool ShouldIncreaseLineIndent()
		{
			return false;
		}
		
		protected virtual bool ShouldDecreaseLineIndent()
		{
			return false;
		}
		
		void IncreaseLineIndent()
		{
			ModifyLineIndent(true);
		}
		
		void DecreaseLineIndent()
		{
			ModifyLineIndent(false);
		}
		
		void ModifyLineIndent(bool increaseIndent)
		{
			string previousLineIndentation = GetPreviousLineIndentation();
			string indentation = GetNewLineIndentation(previousLineIndentation, increaseIndent);
			string newLineText = indentation + line.Text;
			ReplaceLine(newLineText);
		}
		
		string GetPreviousLineIndentation()
		{
			return GetIndentation(previousLine);
		}
		
		string GetIndentation(IDocumentLine documentLine)
		{
			StringBuilder whitespace = new StringBuilder();			
			foreach (char ch in documentLine.Text) {
				if (Char.IsWhiteSpace(ch)) {
					whitespace.Append(ch);
				} else {
					break;
				}
			}
			return whitespace.ToString();
		}

		string GetNewLineIndentation(string previousLineIndentation, bool increaseIndent)
		{
			string singleIndent = editor.Options.IndentationString;
			if (increaseIndent) {
				return previousLineIndentation + singleIndent;
			} 
			
			// Decrease the new line indentation.
			int decreaselength = previousLineIndentation.Length - singleIndent.Length;
			if (decreaselength < 0) {
				decreaselength = 0;
			}
			return previousLineIndentation.Substring(0, decreaselength);
		}
		
		void ReplaceLine(string newLineText)
		{
			document.Replace(line.Offset, line.Length, newLineText);
		}
	}
}
