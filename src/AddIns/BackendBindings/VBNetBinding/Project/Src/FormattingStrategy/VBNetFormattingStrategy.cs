// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace VBNetBinding.FormattingStrategy
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class VBFormattingStrategy : DefaultFormattingStrategy
	{
		List<VBStatement> statements;
		IList<string> keywords;
		VBStatement interfaceStatement;
		
		List<int> blockTokens = new List<int>(
			new int[] {
				Tokens.Class, Tokens.Module, Tokens.Namespace, Tokens.Interface, Tokens.Structure,
				Tokens.Sub, Tokens.Function, Tokens.Operator,
				Tokens.If, Tokens.For, Tokens.Do, Tokens.While, Tokens.With, Tokens.Select, Tokens.Try,
				Tokens.Property, Tokens.Get, Tokens.Set
			});
		
		bool doCasing;
		bool doInsertion;
		
		public VBFormattingStrategy()
		{
			statements = new List<VBStatement>();
			statements.Add(new VBStatement(@"^if.*?(then|\s+_)$", "^end ?if$", "End If", 1));
			statements.Add(new VBStatement(@"\bclass\s+\w+\s*($|\(\s*Of)", "^end class$", "End Class", 1));
			statements.Add(new VBStatement(@"\bnamespace\s+\w+(\.\w+)*$", "^end namespace$", "End Namespace", 1));
			statements.Add(new VBStatement(@"\bmodule\s+\w+$", "^end module$", "End Module", 1));
			statements.Add(new VBStatement(@"\bstructure\s+\w+\s*($|\(\s*Of)", "^end structure$", "End Structure", 1));
			statements.Add(new VBStatement(@"^while\s+", "^end while$", "End While", 1));
			statements.Add(new VBStatement(@"^select case", "^end select$", "End Select", 1));
			statements.Add(new VBStatement(@"(?<!\b(delegate|mustoverride|declare(\s+(unicode|ansi|auto))?)\s+)\bsub\s+\w+", @"^end\s+sub$", "End Sub", 1));
			statements.Add(new VBStatement(@"(?<!\bmustoverride (readonly |writeonly )?)\bproperty\s+\w+", @"^end\s+property$", "End Property", 1));
			statements.Add(new VBStatement(@"(?<!\b(delegate|mustoverride|declare(\s+(unicode|ansi|auto))?)\s+)\bfunction\s+\w+", @"^end\s+function$", "End Function", 1));
			statements.Add(new VBStatement(@"\boperator(\s*[\+\-\*\/\&\^\>\<\=\\]+\s*|\s+\w+\s*)\(", @"^end\s+operator$", "End Operator", 1));
			statements.Add(new VBStatement(@"\bfor\s+.*?$", "^next( \\w+)?$", "Next", 1));
			statements.Add(new VBStatement(@"^synclock\s+.*?$", "^end synclock$", "End SyncLock", 1));
			statements.Add(new VBStatement(@"^get$", "^end get$", "End Get", 1));
			statements.Add(new VBStatement(@"^with\s+.*?$", "^end with$", "End With", 1));
			statements.Add(new VBStatement(@"^set(\s*\(.*?\))?$", "^end set$", "End Set", 1));
			statements.Add(new VBStatement(@"^try$", "^end try$", "End Try", 1));
			statements.Add(new VBStatement(@"^do\s+.+?$", "^loop$", "Loop", 1));
			statements.Add(new VBStatement(@"^do$", "^loop .+?$", "Loop While ", 1));
			statements.Add(new VBStatement(@"\benum\s+\w+$", "^end enum$", "End Enum", 1));
			interfaceStatement = new VBStatement(@"\binterface\s+\w+\s*($|\(\s*Of)", "^end interface$", "End Interface", 1);
			statements.Add(interfaceStatement);
			statements.Add(new VBStatement(@"\busing\s+", "^end using$", "End Using", 1));
			statements.Add(new VBStatement(@"^#region\s+", "^#end region$", "#End Region", 0));
			
			keywords = new string[] {
				"AddHandler", "AddressOf", "Alias", "And",
				"AndAlso", "As", "Boolean", "ByRef",
				"Byte", "ByVal", "Call", "Case",
				"Catch", "CBool", "CByte", "CChar",
				"CDate", "CDbl", "CDec", "Char",
				"CInt", "Class", "CLng", "CObj",
				"Const", "Continue", "CSByte", "CShort",
				"CSng", "CStr", "CType", "CUInt",
				"CULng", "CUShort", "Date", "Decimal",
				"Declare", "Default", "Delegate", "Dim",
				"DirectCast", "Do", "Double", "Each",
				"Else", "ElseIf", "End", "EndIf", // EndIf special case: converted to "End If"
				"Enum", "Erase", "Error", "Event",
				"Exit", "False", "Finally", "For",
				"Friend", "Function", "Get", "GetType",
				"Global", "GoSub", "GoTo", "Handles",
				"If", "Implements", "Imports", "In",
				"Inherits", "Integer", "Interface", "Is",
				"IsNot", "Let", "Lib", "Like",
				"Long", "Loop", "Me", "Mod",
				"Module", "MustInherit", "MustOverride", "MyBase",
				"MyClass", "Namespace", "Narrowing", "New",
				"Next", "Not", "Nothing", "NotInheritable",
				"NotOverridable", "Object", "Of", "On",
				"Operator", "Option", "Optional", "Or",
				"OrElse", "Overloads", "Overridable", "Overrides",
				"ParamArray", "Partial", "Private", "Property",
				"Protected", "Public", "RaiseEvent", "ReadOnly",
				"ReDim", "REM", "RemoveHandler", "Resume",
				"Return", "SByte", "Select", "Set",
				"Shadows", "Shared", "Short", "Single",
				"Static", "Step", "Stop", "String",
				"Structure", "Sub", "SyncLock", "Then",
				"Throw", "To", "True", "Try",
				"TryCast", "TypeOf", "UInteger", "ULong",
				"UShort", "Using", "Variant", "Wend",
				"When", "While", "Widening", "With",
				"WithEvents", "WriteOnly", "Xor",
				// these are not keywords, but context dependend
				"Until", "Ansi", "Unicode", "Region", "Preserve"
			};
		}
		
		public override void FormatLine(TextArea textArea, int lineNr, int cursorOffset, char ch) // used for comment tag formater/inserter
		{
			textArea.Document.UndoStack.StartUndoGroup();
			FormatLineInternal(textArea, lineNr, cursorOffset, ch);
			textArea.Document.UndoStack.EndUndoGroup();
		}
		
		void FormatLineInternal(TextArea textArea, int lineNr, int cursorOffset, char ch)
		{
			string terminator = textArea.TextEditorProperties.LineTerminator;
			doCasing = PropertyService.Get("VBBinding.TextEditor.EnableCasing", true);
			doInsertion = PropertyService.Get("VBBinding.TextEditor.EnableEndConstructs", true);
			
			if (lineNr > 0)
			{
				LineSegment curLine = textArea.Document.GetLineSegment(lineNr);
				LineSegment lineAbove = lineNr > 0 ? textArea.Document.GetLineSegment(lineNr - 1) : null;
				
				string curLineText = textArea.Document.GetText(curLine.Offset, curLine.Length);
				string lineAboveText = lineAbove == null ? "" : textArea.Document.GetText(lineAbove);
				
				if (ch == '\'') {
					curLineText   = textArea.Document.GetText(curLine);
					
					if (curLineText != null && curLineText.EndsWith("'''") && (lineAboveText == null || !lineAboveText.Trim().StartsWith("'''"))) {
						string indentation = base.GetIndentation(textArea, lineNr);
						object member = GetMemberAfter(textArea, lineNr);
						if (member != null) {
							StringBuilder sb = new StringBuilder();
							sb.Append(" <summary>");
							sb.Append(terminator);
							sb.Append(indentation);
							sb.Append("''' ");
							sb.Append(terminator);
							sb.Append(indentation);
							sb.Append("''' </summary>");
							
							if (member is IMethod) {
								IMethod method = (IMethod)member;
								if (method.Parameters != null && method.Parameters.Count > 0) {
									for (int i = 0; i < method.Parameters.Count; ++i) {
										sb.Append(terminator);
										sb.Append(indentation);
										sb.Append("''' <param name=\"");
										sb.Append(method.Parameters[i].Name);
										sb.Append("\"></param>");
									}
								}
								if (method.ReturnType != null && !method.IsConstructor && method.ReturnType.FullyQualifiedName != "System.Void") {
									sb.Append(terminator);
									sb.Append(indentation);
									sb.Append("''' <returns></returns>");
								}
							}
							textArea.Document.Insert(cursorOffset, sb.ToString());
							
							textArea.Refresh();
							textArea.Caret.Position = textArea.Document.OffsetToPosition(cursorOffset + indentation.Length + "/// ".Length + " <summary>".Length + terminator.Length);
						}
					}
					return;
				}
				
				if (ch == '\n' && lineAboveText != null)
				{
					string texttoreplace = lineAboveText;
					// remove string content
					MatchCollection strmatches = Regex.Matches(texttoreplace, "\"[^\"]*?\"", RegexOptions.Singleline);
					foreach (Match match in strmatches)
					{
						texttoreplace = texttoreplace.Remove(match.Index, match.Length).Insert(match.Index, new String('-', match.Length));
					}
					// remove comments
					texttoreplace = Regex.Replace(texttoreplace, "'.*$", "", RegexOptions.Singleline);
					
					if (doCasing)
					{
						foreach (string keyword in keywords) {
							string regex = "\\b" + keyword + "\\b"; // \b = word border
							MatchCollection matches = Regex.Matches(texttoreplace, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
							foreach (Match match in matches) {
								if (keyword == "EndIf") // special case
									textArea.Document.Replace(lineAbove.Offset + match.Index, match.Length, "End If");
								else
									textArea.Document.Replace(lineAbove.Offset + match.Index, match.Length, keyword);
							}
						}
					}
					
					if (doInsertion)
					{
						if (Regex.IsMatch(texttoreplace.Trim(), @"^If .*[^_]$", RegexOptions.IgnoreCase)) {
							if (false == Regex.IsMatch(texttoreplace, @"\bthen\b", RegexOptions.IgnoreCase)) {
								string specialThen = "Then"; // do special check in cases like If t = True' comment
								if (textArea.Document.GetCharAt(lineAbove.Offset + texttoreplace.Length) == '\'')
									specialThen += " ";
								if (textArea.Document.GetCharAt(lineAbove.Offset + texttoreplace.Length - 1) != ' ')
									specialThen = " " + specialThen;
								
								textArea.Document.Insert(lineAbove.Offset + texttoreplace.Length, specialThen);
								texttoreplace += specialThen;
							}
						}
						foreach (VBStatement statement_ in statements) {
							VBStatement statement = statement_; // allow passing statement byref
							if (Regex.IsMatch(texttoreplace.Trim(), statement.StartRegex, RegexOptions.IgnoreCase)) {
								string indentation = GetIndentation(textArea, lineNr - 1);
								if (IsEndStatementNeeded(textArea, ref statement, lineNr)) {
									textArea.Document.Replace(curLine.Offset, curLine.Length, terminator + indentation + statement.EndStatement);
								}
								for (int i = 0; i < statement.IndentPlus; i++) {
									indentation += Tab.GetIndentationString(textArea.Document);
								}
								
								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText.Trim());
								textArea.Caret.Column = indentation.Length;
								return;
							}
						}
						
						// fix for SD2-1284
						string prevLineText = textArea.Document.GetText(lineAbove);
						string prevLineText2 = (lineNr > 1) ? textArea.Document.GetText(textArea.Document.GetLineSegment(lineNr - 2)) : "";
						if (StripComment(prevLineText.ToLowerInvariant()).Trim(' ', '\t', '\n').StartsWith("case")) {
							string indentation = GetIndentation(textArea, lineNr - 1) + Tab.GetIndentationString(textArea.Document);
							textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText.Trim());
							SmartIndentInternal(textArea, lineNr - 1, lineNr);
							textArea.Caret.Column = GetIndentation(textArea, lineNr).Length;
							return;
						}
					}
					
					if (IsInString(lineAboveText))
					{
						if (IsFinishedString(curLineText)) {
							textArea.Document.Insert(lineAbove.Offset + lineAbove.Length,
							                         "\" & _");
							curLine = textArea.Document.GetLineSegment(lineNr);
							textArea.Document.Insert(curLine.Offset, "\"");
							
							if (IsElseConstruct(lineAboveText))
								SmartIndentLine(textArea, lineNr - 1);
							textArea.Caret.Column = SmartIndentLine(textArea, lineNr) + 1;
						} else {
							textArea.Document.Insert(lineAbove.Offset + lineAbove.Length,
							                         "\"");
							if (IsElseConstruct(lineAboveText))
								SmartIndentLine(textArea, lineNr - 1);
							textArea.Caret.Column = SmartIndentLine(textArea, lineNr);
						}
					}
					else
					{
						string indent = GetIndentation(textArea, lineNr - 1);
						if (indent.Length > 0) {
							string newLineText = indent + TextUtilities.GetLineAsString(textArea.Document, lineNr).Trim();
							curLine = textArea.Document.GetLineSegment(lineNr);
							textArea.Document.Replace(curLine.Offset, curLine.Length, newLineText);
						}
						if (IsElseConstruct(lineAboveText))
							SmartIndentLine(textArea, lineNr - 1);
						textArea.Caret.Column = indent.Length;
					}
				}
				else if(ch == '>')
				{
					if (IsInsideDocumentationComment(textArea, curLine, cursorOffset))
					{
						curLineText  = textArea.Document.GetText(curLine);
						int column = textArea.Caret.Offset - curLine.Offset;
						int index = Math.Min(column - 1, curLineText.Length - 1);
						
						while (index > 0 && curLineText[index] != '<') {
							--index;
							if(curLineText[index] == '/')
								return; // the tag was an end tag or already
						}
						
						if (index > 0) {
							StringBuilder commentBuilder = new StringBuilder("");
							for (int i = index; i < curLineText.Length && i < column && !Char.IsWhiteSpace(curLineText[i]); ++i) {
								commentBuilder.Append(curLineText[ i]);
							}
							string tag = commentBuilder.ToString().Trim();
							if (!tag.EndsWith(">")) {
								tag += ">";
							}
							if (!tag.StartsWith("/")) {
								textArea.Document.Insert(textArea.Caret.Offset, "</" + tag.Substring(1));
							}
						}
					}
				}
			}
		}
		
		bool IsElseConstruct(string line)
		{
			string t = StripComment(line).ToLowerInvariant();
			if (t.StartsWith("case ")) return true;
			if (t == "else" || t.StartsWith("elseif ")) return true;
			if (t == "catch" || t.StartsWith("catch ")) return true;
			if (t == "finally") return true;
			
			return false;
		}
		
		bool IsInString(string start)
		{
			bool inString = false;
			for (int i = 0; i < start.Length; i++) {
				if (start[i] == '"')
					inString = !inString;
				if (!inString && start[i] == '\'')
					return false;
			}
			return inString;
		}
		
		bool IsFinishedString(string end)
		{
			bool inString = true;
			for (int i = 0; i < end.Length; i++) {
				if (end[i] == '"')
					inString = !inString;
				if (!inString && end[i] == '\'')
					break;
			}
			return !inString;
		}
		
		bool IsEndStatementNeeded(TextArea textArea, ref VBStatement statement, int lineNr)
		{
			Stack<Token> tokens = new Stack<Token>();
			List<Token> missingEnds = new List<Token>();
			
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(textArea.Document.TextContent));
			
			Token currentToken = null;
			Token prevToken = null;

			while ((currentToken = lexer.NextToken()).kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				
				if (IsBlockStart(lexer, currentToken, prevToken)) {
					tokens.Push(currentToken);
				}
				
				if (IsBlockEnd(currentToken, prevToken)) {
					while (tokens.Count > 0 && !IsMatchingEnd(tokens.Peek(), currentToken)) {
						Token t = null;
						missingEnds.Add(t = tokens.Pop());
					}
					if (tokens.Count != 0) {
						if (IsMatchingEnd(tokens.Peek(), currentToken)) {
							tokens.Pop();
						}
					}
				}
				
				prevToken = currentToken;
			}
			
			if (missingEnds.Count > 0) {
				return GetClosestMissing(missingEnds, statement, lineNr) != null;
			} else
				return false;
		}
		
		Token GetClosestMissing(List<Token> missingEnds, VBStatement statement, int lineNr)
		{
			Token closest = null;
			int diff = 0;
			
			foreach (Token t in missingEnds) {
				if (IsMatchingStatement(t, statement) && ((diff = lineNr - t.line + 1) > -1)) {
					if (closest == null)
						closest = t;
					else {
						if (diff < lineNr - closest.line + 1)
							closest = t;
					}
				}
			}
			return closest;
		}
		
		bool IsMatchingEnd(Token begin, Token end)
		{
			if (begin.kind == end.kind)
				return true;
			
			if (begin.kind == Tokens.For && end.kind == Tokens.Next)
				return true;
			
			if (begin.kind == Tokens.Do && end.kind == Tokens.Loop)
				return true;
			
			return false;
		}
		
		bool IsMatchingStatement(Token token, VBStatement statement)
		{
			// funktioniert noch nicht!
			
			if (token.val == "For" && statement.EndStatement == "Next")
				return true;
			
			if (token.val == "Do" && statement.EndStatement.StartsWith("Loop"))
				return true;
			
			bool empty = !string.IsNullOrEmpty(token.val);
			bool match = statement.EndStatement.IndexOf(token.val, StringComparison.InvariantCultureIgnoreCase) != -1;
			
			return empty && match;
		}
		
		string StripComment(string text)
		{
			return Regex.Replace(text, "'.*$", "", RegexOptions.Singleline).Trim();
		}
		
		bool IsInsideDocumentationComment(TextArea textArea, LineSegment curLine, int cursorOffset)
		{
			for (int i = curLine.Offset; i < cursorOffset; ++i) {
				char ch = textArea.Document.GetCharAt(i);
				if (ch == '"') {
					return false;
				}
				if (ch == '\'' && i + 2 < cursorOffset && textArea.Document.GetCharAt(i + 1) == '\'' && textArea.Document.GetCharAt(i + 2) == '\'')
				{
					return true;
				}
			}
			return false;
		}
		
		public override void IndentLines(TextArea textArea, int begin, int end)
		{
			if (textArea.Document.TextEditorProperties.IndentStyle != IndentStyle.Smart) {
				base.IndentLines(textArea, begin, end);
				return;
			}
			
			SmartIndentInternal(textArea, begin, end);
		}
		
		int SmartIndentInternal(TextArea textArea, int begin, int end)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(textArea.Document.TextContent));
			
			int indentation = 0;
			
			int oldLine = 0;
			
			bool inInterface = false;
			bool isMustOverride = false;
			bool isDeclare = false;
			bool isDelegate = false;
			
			Token currentToken = null;
			Token prevToken = null;
			
			while ((currentToken = lexer.NextToken()).kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				
				if (currentToken.kind == Tokens.MustOverride)
					isMustOverride = true;
				
				if (currentToken.kind == Tokens.Delegate)
					isDelegate = true;
				
				if (currentToken.kind == Tokens.Declare)
					isDeclare = true;
				
				if (currentToken.kind == Tokens.EOL)
					isDelegate = isDeclare = isMustOverride = false;
				
				if (IsSpecialCase(currentToken, prevToken)) {
					ApplyToRange(textArea, ref indentation, oldLine, currentToken.line - 1, begin, end);
					indentation--;
					ApplyToRange(textArea, ref indentation, currentToken.line - 1, currentToken.line, begin, end);
					indentation++;
					
					oldLine = currentToken.line;
				}
				
				if (IsBlockEnd(currentToken, prevToken)) {
					ApplyToRange(textArea, ref indentation, oldLine, currentToken.line - 1, begin, end);
					
					if (currentToken.kind == Tokens.Interface)
						inInterface = false;
					
					if (!inInterface && !isMustOverride && !isDeclare && !isDelegate) {
						indentation--;
						
						if (currentToken.kind == Tokens.Select)
							indentation--;
					}
					
					oldLine = currentToken.line - 1;
				}
				
				if (IsBlockStart(lexer, currentToken, prevToken)) {
					ApplyToRange(textArea, ref indentation, oldLine, currentToken.line, begin, end);
					
					if (!inInterface && !isMustOverride && !isDeclare && !isDelegate) {
						indentation++;
						
						if (currentToken.kind == Tokens.Select)
							indentation++;
					}
					
					if (currentToken.kind == Tokens.Interface)
						inInterface = true;
					
					oldLine = currentToken.line;
				}
				
				prevToken = currentToken;
			}
			
			// do last indent step
			ApplyToRange(textArea, ref indentation, oldLine, prevToken.line, begin, end);
			
			return indentation;
		}
		
		bool IsBlockStart(ILexer lexer, Token current, Token prev)
		{
			if (blockTokens.Contains(current.kind)) {
				if (current.kind == Tokens.If) {
					if (prev.kind != Tokens.EOL)
						return false;
					
					lexer.StartPeek();
					
					Token currentToken = null;
					
					while ((currentToken = lexer.Peek()).kind != Tokens.EOL) {
						if (currentToken.kind == Tokens.Then) {
							if (lexer.Peek().kind == Tokens.EOL)
								return true;
							else
								return false;
						}
					}
				}
				
				if (current.kind == Tokens.Function) {
					lexer.StartPeek();
					
					if (lexer.Peek().kind == Tokens.OpenParenthesis)
						return false;
				}
				
				if (current.kind == Tokens.With && prev.kind != Tokens.EOL)
					return false;
				
				if (current.kind == Tokens.While && (prev.kind == Tokens.Skip || prev.kind == Tokens.Take))
					return false;
				
				if (current.kind == Tokens.Select && prev.kind != Tokens.EOL)
					return false;
				
				if (current.kind == Tokens.Class || current.kind == Tokens.Structure) {
					lexer.StartPeek();
					
					Token t = lexer.Peek();
					
					if (t.kind == Tokens.CloseParenthesis || t.kind == Tokens.CloseCurlyBrace || t.kind == Tokens.Comma)
						return false;
				}
				
				if (current.kind == Tokens.Module) {
					lexer.StartPeek();
					
					Token t = lexer.Peek();
					
					if (t.kind == Tokens.Colon)
						return false;
				}
				
				if (prev.kind == Tokens.End ||
				    prev.kind == Tokens.Loop ||
				    prev.kind == Tokens.Exit ||
				    prev.kind == Tokens.Continue ||
				    prev.kind == Tokens.Resume ||
				    prev.kind == Tokens.GoTo ||
				    prev.kind == Tokens.Do)
					return false;
				else
					return true;
			}
			
			return false;
		}
		
		bool IsBlockEnd(Token current, Token prev)
		{
			if (current.kind == Tokens.Next) {
				if (prev.kind == Tokens.Resume)
					return false;
				else
					return true;
			}
			
			if (current.kind == Tokens.Loop)
				return true;
			
			if (blockTokens.Contains(current.kind)) {
				if (prev.kind == Tokens.End)
					return true;
				else
					return false;
			}
			
			return false;
		}
		
		bool IsSpecialCase(Token current, Token prev)
		{
			switch (current.kind) {
				case Tokens.Else:
					return true;
				case Tokens.Case:
					if (prev.kind == Tokens.Select)
						return false;
					else
						return true;
				case Tokens.ElseIf:
					return true;
				case Tokens.Catch:
					return true;
				case Tokens.Finally:
					return true;
			}
			
			return false;
		}
		
		bool multiLine = false;
		bool otherMultiLine = false;

		void ApplyToRange(TextArea textArea, ref int indentation, int begin, int end, int selBegin, int selEnd)
		{
			bool useSpaces = textArea.TextEditorProperties.ConvertTabsToSpaces;
			int indentationSize = textArea.TextEditorProperties.IndentationSize;
			int tabSize = textArea.TextEditorProperties.TabIndent;
			int spaces = indentationSize * (indentation < 0 ? 0 : indentation);
			int tabs = 0;
			
			if (begin >= end) {
				LineSegment curLine = textArea.Document.GetLineSegment(begin);
				string lineText = textArea.Document.GetText(curLine).Trim(' ', '\t', '\n');
				
				string newLine = new string('\t', tabs) + new string(' ', spaces) + lineText;
				
				if (begin >= selBegin && begin <= selEnd)
					SmartReplaceLine(textArea.Document, curLine, newLine);
			}
			
			for (int i = begin; i < end; i++) {
				LineSegment curLine = textArea.Document.GetLineSegment(i);
				string lineText = textArea.Document.GetText(curLine).Trim(' ', '\t', '\n');
				
				string noComments = StripComment(lineText).TrimEnd(' ', '\t', '\n');
				
				
				if (otherMultiLine && noComments == "}") {
					indentation--;
					otherMultiLine = false;
				}
				
				spaces = indentationSize * (indentation < 0 ? 0 : indentation);
				tabs = 0;
				
				if (!useSpaces) {
					tabs = (int)(spaces / tabSize);
					spaces %= tabSize;
				}

				
				string newLine = new string('\t', tabs) + new string(' ', spaces) + lineText;
				
				if (noComments.EndsWith("_") && !IsStatement(noComments) && !otherMultiLine) {
					otherMultiLine = true;
					indentation++;
				}
				
				if (noComments.EndsWith("_") && IsStatement(noComments)) {
					multiLine = true;
					indentation++;
				}
				
				if (!noComments.EndsWith("_") && multiLine) {
					indentation--;
					multiLine = false;
				}
				
				if (!noComments.EndsWith("_") && otherMultiLine) {
					indentation--;
					otherMultiLine = false;
				}
				
				if (i >= selBegin && i <= selEnd)
					SmartReplaceLine(textArea.Document, curLine, newLine);
				
				LoggingService.Debug("'" + newLine + "'");
			}
		}
		
		bool IsStatement(string text)
		{
			foreach (VBStatement s in this.statements) {
				if (Regex.IsMatch(text, s.StartRegex, RegexOptions.IgnoreCase))
					return true;
			}
			
			return false;
		}

		protected override int SmartIndentLine(TextArea textArea, int line)
		{
			if (line <= 0)
				return AutoIndentLine(textArea, line);
			return SmartIndentInternal(textArea, line, line);
		}
		
		/// <summary>
		/// Gets the next member after the specified caret position.
		/// </summary>
		object GetMemberAfter(TextArea textArea, int caretLine)
		{
			string fileName = textArea.MotherTextEditorControl.FileName;
			object nextElement = null;
			if (fileName != null && fileName.Length > 0 ) {
				ParseInformation parseInfo = ParserService.ParseFile(fileName, textArea.Document.TextContent);
				if (parseInfo != null) {
					ICompilationUnit currentCompilationUnit = parseInfo.BestCompilationUnit;
					if (currentCompilationUnit != null) {
						IClass currentClass = currentCompilationUnit.GetInnermostClass(caretLine, 0);
						int nextElementLine = int.MaxValue;
						if (currentClass == null) {
							foreach (IClass c in currentCompilationUnit.Classes) {
								if (c.Region.BeginLine < nextElementLine && c.Region.BeginLine > caretLine) {
									nextElementLine = c.Region.BeginLine;
									nextElement = c;
								}
							}
						} else {
							foreach (IClass c in currentClass.InnerClasses) {
								if (c.Region.BeginLine < nextElementLine && c.Region.BeginLine > caretLine) {
									nextElementLine = c.Region.BeginLine;
									nextElement = c;
								}
							}
							foreach (IMember m in currentClass.Methods) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
							foreach (IMember m in currentClass.Properties) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
							foreach (IMember m in currentClass.Fields) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
							foreach (IMember m in currentClass.Events) {
								if (m.Region.BeginLine < nextElementLine && m.Region.BeginLine > caretLine) {
									nextElementLine = m.Region.BeginLine;
									nextElement = m;
								}
							}
						}
					}
				}
			}
			return nextElement;
		}
		
		#region SearchBracket
		public override int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			bool inString  = false;
			char ch;
			int brackets = -1;
			for (int i = offset; i > 0; --i) {
				ch = document.GetCharAt(i);
				if (ch == openBracket && !inString) {
					++brackets;
					if (brackets == 0) return i;
				} else if (ch == closingBracket && !inString) {
					--brackets;
				} else if (ch == '"') {
					inString = !inString;
				} else if (ch == '\n') {
					int lineStart = ScanLineStart(document, i);
					if (lineStart >= 0) { // line could have a comment
						inString = false;
						for (int j = lineStart; j < i; ++j) {
							ch = document.GetCharAt(j);
							if (ch == '"') inString = !inString;
							if (ch == '\'' && !inString) {
								// comment found!
								// Skip searching in the comment:
								i = j;
								break;
							}
						}
					}
					inString = false;
				}
			}
			return -1;
		}
		
		static int ScanLineStart(IDocument document, int offset)
		{
			bool hasComment = false;
			for (int i = offset - 1; i > 0; --i) {
				char ch = document.GetCharAt(i);
				if (ch == '\n') {
					if (!hasComment) return -1;
					return i + 1;
				} else if (ch == '\'') {
					hasComment = true;
				}
			}
			return 0;
		}
		
		public override int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			bool inString  = false;
			bool inComment = false;
			int  brackets  = 1;
			for (int i = offset; i < document.TextLength; ++i) {
				char ch = document.GetCharAt(i);
				if (ch == '\n') {
					inString  = false;
					inComment = false;
				}
				if (inComment) continue;
				if (ch == '"') inString = !inString;
				if (inString)  continue;
				if (ch == '\'') {
					inComment = true;
				} else if (ch == openBracket) {
					++brackets;
				} else if (ch == closingBracket) {
					--brackets;
					if (brackets == 0) return i;
				}
			}
			return -1;
		}
		#endregion
	}
}
