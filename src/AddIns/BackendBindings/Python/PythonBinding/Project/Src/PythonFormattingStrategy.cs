// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.PythonBinding
{
	public class PythonFormattingStrategy : DefaultFormattingStrategy
	{
		public PythonFormattingStrategy()
		{
		}
		
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			if (line.LineNumber == 1) {
				base.IndentLine(editor, line);
				return;
			}
			
			IDocument document = editor.Document;
			IDocumentLine previousLine = document.GetLine(line.LineNumber - 1);
			string previousLineText = previousLine.Text.Trim();
			
			if (previousLineText.EndsWith(":")) {
				IncreaseLineIndent(editor, line);
			} else if (previousLineText == "pass") {
				DecreaseLineIndent(editor, line);
			} else if ((previousLineText == "return") || (previousLineText.StartsWith("return "))) {
				DecreaseLineIndent(editor, line);
			} else if ((previousLineText == "raise") || (previousLineText.StartsWith("raise "))) {
				DecreaseLineIndent(editor, line);
			} else if (previousLineText == "break") {
				DecreaseLineIndent(editor, line);
			} else {
				base.IndentLine(editor, line);
			}
		}
		
		void IncreaseLineIndent(ITextEditor editor, IDocumentLine line)
		{
			ModifyLineIndent(editor, line, true);
		}
		
		void DecreaseLineIndent(ITextEditor editor, IDocumentLine line)
		{
			ModifyLineIndent(editor, line, false);
		}
		
		void ModifyLineIndent(ITextEditor editor, IDocumentLine line, bool increaseIndent)
		{
			string indentation = GetLineIndentation(editor, line.LineNumber - 1);
			indentation = GetNewLineIndentation(indentation, editor.Options.IndentationString, increaseIndent);
			string newIndentedText = indentation + line.Text;
			editor.Document.Replace(line.Offset, line.Length, newIndentedText);
		}
		
		string GetLineIndentation(ITextEditor editor, int line)
		{
			IDocumentLine documentLine = editor.Document.GetLine(line);
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
		
		string GetNewLineIndentation(string previousLineIndentation, string singleIndent, bool increaseIndent)
		{
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
	}
}
