// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.RubyBinding
{
	public class RubyFormattingStrategy : DefaultFormattingStrategy
	{
		TextArea textArea;
		int line;
		IDocument document;
		string previousLineText;
		List<string> decreaseLineIndentStatements = new List<string>();
		List<string> decreaseLineIndentStartsWithStatements = new List<string>();

		List<string> increaseLineIndentStatements = new List<string>();
		List<string> increaseLineIndentStartsWithStatements = new List<string>();
		List<string> increaseLineIndentEndsWithStatements = new List<string>();
		List<string> increaseLineIndentContainsStatements = new List<string>();
		
		public RubyFormattingStrategy()
		{
			CreateDecreaseLineIndentStatements();
			CreateDecreaseLineIndentStartsWithStatements();
			CreateIncreaseLineIndentStatements();
			CreateIncreaseLineIndentStartsWithStatements();
			CreateIncreaseLineIndentEndsWithStatements();
			CreateIncreaseLineIndentContainsStatements();
		}
		
		void CreateDecreaseLineIndentStatements()
		{
			decreaseLineIndentStatements.Add("break");
			decreaseLineIndentStatements.Add("return");
			decreaseLineIndentStatements.Add("raise");
		}
		
		void CreateDecreaseLineIndentStartsWithStatements()
		{
			decreaseLineIndentStartsWithStatements.Add("return ");
			decreaseLineIndentStartsWithStatements.Add("raise ");
		}
		
		void CreateIncreaseLineIndentStatements()
		{
			increaseLineIndentStatements.Add("else");
			increaseLineIndentStatements.Add("begin");
			increaseLineIndentStatements.Add("rescue");
			increaseLineIndentStatements.Add("ensure");
		}
		
		void CreateIncreaseLineIndentStartsWithStatements()
		{
			increaseLineIndentStartsWithStatements.Add("if ");
			increaseLineIndentStartsWithStatements.Add("def ");
			increaseLineIndentStartsWithStatements.Add("class ");
			increaseLineIndentStartsWithStatements.Add("while ");
			increaseLineIndentStartsWithStatements.Add("elsif ");
			increaseLineIndentStartsWithStatements.Add("loop ");
			increaseLineIndentStartsWithStatements.Add("unless ");
			increaseLineIndentStartsWithStatements.Add("until ");
			increaseLineIndentStartsWithStatements.Add("for ");
			increaseLineIndentStartsWithStatements.Add("rescue ");			
			increaseLineIndentStartsWithStatements.Add("module ");
			increaseLineIndentStartsWithStatements.Add("when ");
			increaseLineIndentStartsWithStatements.Add("case ");
		}
		
		void CreateIncreaseLineIndentEndsWithStatements()
		{
			increaseLineIndentEndsWithStatements.Add(" then");
			increaseLineIndentEndsWithStatements.Add(" do");
			increaseLineIndentEndsWithStatements.Add(" {");
		}
		void CreateIncreaseLineIndentContainsStatements()
		{
			increaseLineIndentContainsStatements.Add(" case ");
		}

		protected override int SmartIndentLine(TextArea textArea, int line)
		{
			this.textArea = textArea;
			this.line = line;
			this.document = textArea.Document;
			
			GetPreviousLineText();
			
			if (ShouldDecreaseLineIndent()) {
				return DecreaseLineIndent();
			} else if (ShouldIncreaseLineIndent()) {
				return IncreaseLineIndent();
			}
			return base.SmartIndentLine(textArea, line);
		}
		
		void GetPreviousLineText()
		{
			LineSegment previousLine = document.GetLineSegment(line - 1);
			this.previousLineText = document.GetText(previousLine).Trim();
		}
		
		bool ShouldIncreaseLineIndent()
		{
			if (increaseLineIndentStatements.Contains(previousLineText)) {
				return true;
			}
			if (PreviousLineStartsWith(increaseLineIndentStartsWithStatements)) {
				return true;
			}
			if (PreviousLineContains(increaseLineIndentContainsStatements)) {
				return true;
			}
			return PreviousLineEndsWith(increaseLineIndentEndsWithStatements);
		}
		
		bool ShouldDecreaseLineIndent()
		{
			if (decreaseLineIndentStatements.Contains(previousLineText)) {
				return true;
			}
			return PreviousLineStartsWith(decreaseLineIndentStartsWithStatements);
		}
		
		bool PreviousLineStartsWith(List<string> items)
		{
			foreach (string item in items) {
				if (previousLineText.StartsWith(item)) {
					return true;
				}
			}
			return false;
		}
		
		bool PreviousLineEndsWith(List<string> items)
		{
			foreach (string item in items) {
				if (previousLineText.EndsWith(item)) {
					return true;
				}
			}
			return false;
		}
		
		bool PreviousLineContains(List<string> items)
		{
			foreach (string item in items) {
				if (previousLineText.Contains(item)) {
					return true;
				}
			}
			return false;
		}
		
		int IncreaseLineIndent()
		{
			return ModifyLineIndent(true);
		}
		
		int DecreaseLineIndent()
		{
			return ModifyLineIndent(false);
		}
		
		int ModifyLineIndent(bool increaseIndent)
		{
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
