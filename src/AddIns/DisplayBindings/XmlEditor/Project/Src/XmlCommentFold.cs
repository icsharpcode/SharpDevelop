// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.SharpDevelop.Dom;

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
