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
using System.Text;
using ICSharpCode.NRefactory.Editor;
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
			previousLine = document.GetLineByNumber(lineNumber);
			previousLineText = document.GetText(previousLine).Trim();
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
			string newLineText = indentation + document.GetText(line);
			ReplaceLine(newLineText);
		}
		
		string GetPreviousLineIndentation()
		{
			return GetIndentation(previousLine);
		}
		
		string GetIndentation(IDocumentLine documentLine)
		{
			StringBuilder whitespace = new StringBuilder();
			foreach (char ch in document.GetText(documentLine)) {
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
