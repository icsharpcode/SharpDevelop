// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpBinding.FormattingStrategy
{
	public sealed class IndentationSettings
	{
		public string IndentString = "\t";
		/// <summary>Leave empty lines empty.</summary>
		public bool LeaveEmptyLines = true;
	}
	
	public sealed class IndentationReformatter
	{
		public struct Block
		{
			public string OuterIndent;
			public string InnerIndent;
			public string LastWord;
			public char Bracket;
			public bool Continuation;
			public bool OneLineBlock;
			public int StartLine;
			
			public void Indent(IndentationSettings set)
			{
				Indent(set, set.IndentString);
			}
			
			public void Indent(IndentationSettings set, string str)
			{
				OuterIndent = InnerIndent;
				InnerIndent += str;
				Continuation = false;
				OneLineBlock = false;
				LastWord = "";
			}
		}
		
		StringBuilder wordBuilder;
		Stack<Block> blocks; // blocks contains all blocks outside of the current
		Block block;  // block is the current block
		
		bool inString = false;
		bool inChar   = false;
		bool verbatim = false;
		bool escape   = false;
		
		bool lineComment  = false;
		bool blockComment = false;
		
		char lastRealChar = ' '; // last non-comment char
		
		public void Reformat(IDocumentAccessor doc, IndentationSettings set)
		{
			Init();
			
			while (doc.Next()) {
				Step(doc, set);
			}
		}
		
		public void Init()
		{
			wordBuilder = new StringBuilder();
			blocks = new Stack<Block>();
			block = new Block();
			block.InnerIndent = "";
			block.OuterIndent = "";
			block.Bracket = '{';
			block.Continuation = false;
			block.LastWord = "";
			block.OneLineBlock = false;
			block.StartLine = 0;
			
			inString = false;
			inChar   = false;
			verbatim = false;
			escape   = false;
			
			lineComment  = false;
			blockComment = false;
			
			lastRealChar = ' '; // last non-comment char
		}
		
		public void Step(IDocumentAccessor doc, IndentationSettings set)
		{
			string line = doc.Text;
			if (set.LeaveEmptyLines && line.Length == 0) return; // leave empty lines empty
			line = line.TrimStart();
			
			StringBuilder indent = new StringBuilder();
			if (line.Length == 0) {
				// Special treatment for empty lines:
				if (blockComment || (inString && verbatim))
					return;
				indent.Append(block.InnerIndent);
				if (block.OneLineBlock)
					indent.Append(set.IndentString);
				if (block.Continuation)
					indent.Append(set.IndentString);
				if (doc.Text != indent.ToString())
					doc.Text = indent.ToString();
				return;
			}
			
			if (TrimEnd(doc))
				line = doc.Text.TrimStart();
			
			Block oldBlock = block;
			bool startInComment = blockComment;
			bool startInString = (inString && verbatim);
			
			#region Parse char by char
			lineComment = false;
			inChar = false;
			escape = false;
			if (!verbatim) inString = false;
			
			lastRealChar = '\n';
			
			char lastchar = ' ';
			char c = ' ';
			char nextchar = line[0];
			for (int i = 0; i < line.Length; i++) {
				if (lineComment) break; // cancel parsing current line
				
				lastchar = c;
				c = nextchar;
				if (i + 1 < line.Length)
					nextchar = line[i + 1];
				else
					nextchar = '\n';
				
				if (escape) {
					escape = false;
					continue;
				}
				
				#region Check for comment/string chars
				switch (c) {
					case '/':
						if (blockComment && lastchar == '*')
							blockComment = false;
						if (!inString && !inChar) {
							if (!blockComment && nextchar == '/')
								lineComment = true;
							if (!lineComment && nextchar == '*')
								blockComment = true;
						}
						break;
					case '#':
						if (!(inChar || blockComment || inString))
							lineComment = true;
						break;
					case '"':
						if (!(inChar || lineComment || blockComment)) {
							inString = !inString;
							if (!inString && verbatim) {
								if (nextchar == '"') {
									escape = true; // skip escaped quote
									inString = true;
								} else {
									verbatim = false;
								}
							} else if (inString && lastchar == '@') {
								verbatim = true;
							}
						}
						break;
					case '\'':
						if (!(inString || lineComment || blockComment)) {
							inChar = !inChar;
						}
						break;
					case '\\':
						if ((inString && !verbatim) || inChar)
							escape = true; // skip next character
						break;
				}
				#endregion
				
				if (lineComment || blockComment || inString || inChar) {
					if (wordBuilder.Length > 0)
						block.LastWord = wordBuilder.ToString();
					wordBuilder.Length = 0;
					continue;
				}
				
				if (!Char.IsWhiteSpace(c) && c != '[' && c != '/') {
					if (block.Bracket == '{')
						block.Continuation = true;
				}
				
				if (Char.IsLetterOrDigit(c)) {
					wordBuilder.Append(c);
				} else {
					if (wordBuilder.Length > 0)
						block.LastWord = wordBuilder.ToString();
					wordBuilder.Length = 0;
				}
				
				#region Push/Pop the blocks
				switch (c) {
					case '{':
						block.OneLineBlock = false;
						blocks.Push(block);
						block.StartLine = doc.LineNumber;
						if (block.LastWord == "switch") {
							block.Indent(set, set.IndentString + set.IndentString);
						} else if (oldBlock.OneLineBlock) {
							// Inside a one-line-block is another statement
							// with a full block: indent the inner full block
							// by one additional level
							block.Indent(set, set.IndentString + set.IndentString);
							block.OuterIndent += set.IndentString;
							// Indent current line if it starts with the '{' character
							if (i == 0) {
								oldBlock.InnerIndent += set.IndentString;
							}
						} else {
							block.Indent(set);
						}
						block.Bracket = '{';
						break;
					case '}':
						while (block.Bracket != '{') {
							if (blocks.Count == 0) break;
							block = blocks.Pop();
						}
						if (blocks.Count == 0) break;
						block = blocks.Pop();
						block.Continuation = false;
						block.OneLineBlock = false;
						break;
					case '(':
					case '[':
						blocks.Push(block);
						if (block.StartLine == doc.LineNumber)
							block.InnerIndent = block.OuterIndent;
						else
							block.StartLine = doc.LineNumber;
						block.Indent(set,
						             (oldBlock.OneLineBlock ? set.IndentString : "") +
						             (oldBlock.Continuation ? set.IndentString : "") +
						             (i == line.Length - 1 ? set.IndentString : new String(' ', i + 1)));
						block.Bracket = c;
						break;
					case ')':
						if (blocks.Count == 0) break;
						if (block.Bracket == '(') {
							block = blocks.Pop();
							if (IsSingleStatementKeyword(block.LastWord))
								block.Continuation = false;
						}
						break;
					case ']':
						if (blocks.Count == 0) break;
						if (block.Bracket == '[')
							block = blocks.Pop();
						break;
					case ';':
					case ',':
						block.Continuation = false;
						block.OneLineBlock = false;
						break;
					case ':':
						if (block.LastWord == "case" || line.StartsWith("case ") || line.StartsWith(block.LastWord + ":")) {
							block.Continuation = false;
							block.OneLineBlock = false;
						}
						break;
				}
				
				if (!Char.IsWhiteSpace(c)) {
					// register this char as last char
					lastRealChar = c;
				}
				#endregion
			}
			#endregion
			
			if (wordBuilder.Length > 0)
				block.LastWord = wordBuilder.ToString();
			wordBuilder.Length = 0;
			
			if (startInString) return;
			if (startInComment && line[0] != '*') return;
			if (doc.Text.StartsWith("//\t") || doc.Text == "//")
				return;
			
			if (line[0] == '}') {
				indent.Append(oldBlock.OuterIndent);
				oldBlock.OneLineBlock = false;
				oldBlock.Continuation = false;
			} else {
				indent.Append(oldBlock.InnerIndent);
			}
			
			if (indent.Length > 0 && oldBlock.Bracket == '(' && line[0] == ')') {
				indent.Remove(indent.Length - 1, 1);
			} else if (indent.Length > 0 && oldBlock.Bracket == '[' && line[0] == ']') {
				indent.Remove(indent.Length - 1, 1);
			}
			
			if (line[0] == ':') {
				oldBlock.Continuation = true;
			} else if (lastRealChar == ':' && indent.Length >= set.IndentString.Length) {
				if (block.LastWord == "case" || line.StartsWith("case ") || line.StartsWith(block.LastWord + ":"))
					indent.Remove(indent.Length - set.IndentString.Length, set.IndentString.Length);
			} else if (lastRealChar == ')') {
				if (IsSingleStatementKeyword(block.LastWord)) {
					block.OneLineBlock = true;
				}
			} else if (lastRealChar == 'e' && block.LastWord == "else") {
				block.OneLineBlock = true;
				block.Continuation = false;
			}
			
			if (doc.ReadOnly) {
				// We can't change the current line, but we should accept the existing
				// indentation if possible (=if the current statement is not a multiline
				// statement).
				if (!oldBlock.Continuation && !oldBlock.OneLineBlock &&
				    oldBlock.StartLine == block.StartLine &&
				    block.StartLine < doc.LineNumber && lastRealChar != ':')
				{
					// use indent StringBuilder to get the indentation of the current line
					indent.Length = 0;
					line = doc.Text; // get untrimmed line
					for (int i = 0; i < line.Length; ++i) {
						if (!Char.IsWhiteSpace(line[i]))
							break;
						indent.Append(line[i]);
					}
					// /* */ multiline comments have an extra space - do not count it
					// for the block's indentation.
					if (startInComment && indent.Length > 0 && indent[indent.Length - 1] == ' ') {
						indent.Length -= 1;
					}
					block.InnerIndent = indent.ToString();
				}
				return;
			}
			
			if (line[0] != '{') {
				if (line[0] != ')' && oldBlock.Continuation && oldBlock.Bracket == '{')
					indent.Append(set.IndentString);
				if (oldBlock.OneLineBlock)
					indent.Append(set.IndentString);
			}
			
			// this is only for blockcomment lines starting with *,
			// all others keep their old indentation
			if (startInComment)
				indent.Append(' ');
			
			if (indent.Length != (doc.Text.Length - line.Length) ||
			    !doc.Text.StartsWith(indent.ToString()) ||
			    Char.IsWhiteSpace(doc.Text[indent.Length]))
			{
				doc.Text = indent.ToString() + line;
			}
		}
		
		bool IsSingleStatementKeyword(string keyword) {
			switch (keyword) {
				case "if":
				case "for":
				case "while":
				case "do":
				case "foreach":
				case "using":
				case "lock":
					return true;
				default:
					return false;
			}
		}
		
		bool TrimEnd(IDocumentAccessor doc)
		{
			string line = doc.Text;
			if (!Char.IsWhiteSpace(line[line.Length - 1])) return false;
			
			// one space after an empty comment is allowed
			if (line.EndsWith("// ") || line.EndsWith("* "))
				return false;
			
			doc.Text = line.TrimEnd();
			return true;
		}
	}
}
