// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.RubyBinding
{
	public class RubyFormattingStrategy : DefaultFormattingStrategy
	{
		ITextEditor textEditor;
		IDocumentLine currentLine;
		IDocumentLine previousLine;
		string previousLineTextTrimmed;
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
		
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			this.textEditor = editor;
			this.currentLine = line;
			
			GetPreviousLineText();
			
			if (ShouldDecreaseLineIndent()) {
				DecreaseLineIndent();
			} else if (ShouldIncreaseLineIndent()) {
				IncreaseLineIndent();
			} else {
				base.IndentLine(editor, line);
			}
		}
		
		void GetPreviousLineText()
		{
			previousLine = textEditor.Document.GetLine(currentLine.LineNumber - 1);
			previousLineTextTrimmed = previousLine.Text.Trim();
		}
		
		bool ShouldIncreaseLineIndent()
		{
			if (increaseLineIndentStatements.Contains(previousLineTextTrimmed)) {
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
			if (decreaseLineIndentStatements.Contains(previousLineTextTrimmed)) {
				return true;
			}
			return PreviousLineStartsWith(decreaseLineIndentStartsWithStatements);
		}
		
		bool PreviousLineStartsWith(List<string> items)
		{
			foreach (string item in items) {
				if (previousLineTextTrimmed.StartsWith(item)) {
					return true;
				}
			}
			return false;
		}
		
		bool PreviousLineEndsWith(List<string> items)
		{
			foreach (string item in items) {
				if (previousLineTextTrimmed.EndsWith(item)) {
					return true;
				}
			}
			return false;
		}
		
		bool PreviousLineContains(List<string> items)
		{
			foreach (string item in items) {
				if (previousLineTextTrimmed.Contains(item)) {
					return true;
				}
			}
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
			string indentation = GetPreviousLineIndentation();
			indentation = GetNewLineIndentation(indentation, textEditor.Options.IndentationString, increaseIndent);
			string newIndentedText = indentation + currentLine.Text;
			textEditor.Document.Replace(currentLine.Offset, currentLine.Length, newIndentedText);
		}
		
		string GetPreviousLineIndentation()
		{
			StringBuilder whitespace = new StringBuilder();			
			foreach (char ch in previousLine.Text) {
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
