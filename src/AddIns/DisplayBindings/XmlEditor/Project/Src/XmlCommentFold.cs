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
using System.Xml;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.XmlEditor
{
	public class XmlCommentFold
	{
		string[] lines = new string[0];
		string comment = String.Empty;
		DomRegion commentRegion;
		string displayText;
		
		const string CommentStartTag = "<!--";
		const string CommentEndTag = "-->";
		
		public XmlCommentFold(XmlTextReader reader)
		{
			ReadComment(reader);
		}
		
		void ReadComment(XmlTextReader reader)
		{
			GetCommentLines(reader);
			GetCommentRegion(reader, lines);
			GetCommentDisplayText(lines);
		}
		
		void GetCommentLines(XmlTextReader reader)
		{
			comment = reader.Value.Replace("\r\n", "\n");
			lines = comment.Split('\n');
		}
		
		void GetCommentRegion(XmlTextReader reader, string[] lines)
		{
			int startColumn = reader.LinePosition - CommentStartTag.Length;
			int startLine = reader.LineNumber;
			
			string lastLine = lines[lines.Length - 1];
			int endColumn = lastLine.Length + startColumn + CommentEndTag.Length;
			int endLine = startLine + lines.Length - 1;
			
			commentRegion = new DomRegion(startLine, startColumn, endLine, endColumn);
		}
		
		void GetCommentDisplayText(string[] lines)
		{
			string firstLine = String.Empty;
			if (lines.Length > 0) {
				firstLine = lines[0];
			}
			displayText = String.Concat("<!-- ", firstLine.Trim(), " -->");
		}
		
		public FoldingRegion CreateFoldingRegion()
		{
			return new FoldingRegion(displayText, commentRegion);
		}
		
		public bool IsSingleLine {
			get { return commentRegion.BeginLine == commentRegion.EndLine; }
		}
	}
}
