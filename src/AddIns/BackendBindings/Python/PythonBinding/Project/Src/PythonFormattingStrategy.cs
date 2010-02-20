// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	public class PythonFormattingStrategy : DefaultFormattingStrategy
	{
		public PythonFormattingStrategy()
		{
		}
		
		protected override int SmartIndentLine(TextArea textArea, int line)
		{
			if (line == 0) {
				return base.SmartIndentLine(textArea, line);
			}
			
			IDocument document = textArea.Document;
			LineSegment previousLine = document.GetLineSegment(line - 1);
			string previousLineText = document.GetText(previousLine).Trim();
			
			if (previousLineText.EndsWith(":")) {
				return IncreaseLineIndent(textArea, line);
			} else if (previousLineText == "pass") {
				return DecreaseLineIndent(textArea, line);
			} else if ((previousLineText == "return") || (previousLineText.StartsWith("return "))) {
				return DecreaseLineIndent(textArea, line);
			} else if ((previousLineText == "raise") || (previousLineText.StartsWith("raise "))) {
				return DecreaseLineIndent(textArea, line);
			} else if (previousLineText == "break") {
				return DecreaseLineIndent(textArea, line);
			}
			return base.SmartIndentLine(textArea, line);
		}
		
		int IncreaseLineIndent(TextArea textArea, int line)
		{
			return ModifyLineIndent(textArea, line, true);
		}
		
		int DecreaseLineIndent(TextArea textArea, int line)
		{
			return ModifyLineIndent(textArea, line, false);
		}
		
		int ModifyLineIndent(TextArea textArea, int line, bool increaseIndent)
		{
			IDocument document = textArea.Document;
			LineSegment currentLine = document.GetLineSegment(line);
			string indentation = GetIndentation(textArea, line - 1);
			indentation = GetNewLineIndentation(indentation, Tab.GetIndentationString(document), increaseIndent);
			string newIndentedText = indentation + document.GetText(currentLine);
			SmartReplaceLine(document, currentLine, newIndentedText);
			return indentation.Length;
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
