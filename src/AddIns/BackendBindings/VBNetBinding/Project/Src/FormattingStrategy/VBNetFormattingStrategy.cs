// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
							if (!Regex.IsMatch(texttoreplace, @"\bthen\b", RegexOptions.IgnoreCase)) {
								string specialThen = "Then"; // do special check in cases like If t = True' comment
								if (textArea.Document.GetCharAt(lineAbove.Offset + texttoreplace.Length) == '\'')
									specialThen += " ";
								if (textArea.Document.GetCharAt(lineAbove.Offset + texttoreplace.Length - 1) != ' ')
									specialThen = " " + specialThen;
								
								textArea.Document.Insert(lineAbove.Offset + texttoreplace.Length, specialThen);
								texttoreplace += specialThen;
							}
						}
						// check #Region statements
						if (Regex.IsMatch(texttoreplace.Trim(), @"^#Region", RegexOptions.IgnoreCase) && LookForEndRegion(textArea)) {
							string indentation = GetIndentation(textArea, lineNr - 1);
							texttoreplace += indentation + "\r\n" + indentation + "#End Region";
							textArea.Document.Replace(curLine.Offset, curLine.Length, texttoreplace);
						}
						foreach (VBStatement statement_ in statements) {
							VBStatement statement = statement_; // allow passing statement byref
							if (Regex.IsMatch(texttoreplace.Trim(), statement.StartRegex, RegexOptions.IgnoreCase)) {
								string indentation = GetIndentation(textArea, lineNr - 1);
								if (IsEndStatementNeeded(textArea, ref statement, lineNr))
									textArea.Document.Replace(curLine.Offset, curLine.Length, terminator + indentation + statement.EndStatement);
								if (!IsInsideInterface(textArea, lineNr) || statement == interfaceStatement) {
									for (int i = 0; i < statement.IndentPlus; i++) {
										indentation += Tab.GetIndentationString(textArea.Document);
									}
								}

								textArea.Document.Replace(curLine.Offset, curLine.Length, indentation + curLineText.Trim());
								textArea.Caret.Column = indentation.Length;
								return;
							}
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
							SmartIndentInternal(textArea, lineNr - 1, lineNr);
						textArea.Caret.Column = GetIndentation(textArea, lineNr).Length;
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
		
		bool LookForEndRegion(TextArea area)
		{
			string lineText = area.Document.GetText(area.Document.GetLineSegment(0));
			int count = 0;
			int lineNr = 0;
			while ((!Regex.IsMatch(lineText, @"^\s*#End\s+Region", RegexOptions.IgnoreCase) || count >= 0) && (lineNr < area.Document.TotalNumberOfLines)) {
				if (Regex.IsMatch(lineText, @"^\s*#Region", RegexOptions.IgnoreCase))
					count++;
				if (Regex.IsMatch(lineText, @"^\s*#End\s+Region", RegexOptions.IgnoreCase))
					count--;
				lineNr++;
				if (lineNr < area.Document.TotalNumberOfLines)
					lineText = area.Document.GetText(area.Document.GetLineSegment(lineNr));
			}
			
			return (count > 0);
		}
		
		bool IsInsideInterface(TextArea area, int lineNr)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(area.Document.TextContent));
			
			Stack<Token> tokens = new Stack<Token>();
			
			Token currentToken = null;
			Token prevToken = null;
			
			while ((currentToken = lexer.NextToken()).Kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				
				if (currentToken.EndLocation.Line <= lineNr &&
				    IsDeclaration(currentToken.Kind) &&
				    IsBlockStart(lexer, currentToken, prevToken)) {
					
					tokens.Push(currentToken);
				}
				
				if (currentToken.EndLocation.Line <= lineNr &&
				    IsDeclaration(currentToken.Kind) &&
				    IsBlockEnd(currentToken, prevToken)) {
					
					if (tokens.Count > 0)
						tokens.Pop();
				}
				
				if (currentToken.EndLocation.Line > lineNr)
					break;
			}
			
			if (tokens.Count > 0)
				return tokens.Pop().Kind == Tokens.Interface;
			
			return false;
		}
		
		bool IsElseConstruct(string line)
		{
			string t = StripComment(line);
			if (t.StartsWith("case ", StringComparison.OrdinalIgnoreCase)) return true;
			if (string.Compare(t, "else", true) == 0 ||
			    t.StartsWith("elseif ", StringComparison.OrdinalIgnoreCase)) return true;
			if (string.Compare(t, "catch", true) == 0 ||
			    t.StartsWith("catch ", StringComparison.OrdinalIgnoreCase)) return true;
			if (string.Compare(t, "finally", true) == 0) return true;
			
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
		
		bool IsDeclaration(int type)
		{
			return (type == Tokens.Class) ||
				(type == Tokens.Module) ||
				(type == Tokens.Structure) ||
				(type == Tokens.Interface);
		}
		
		bool IsEndStatementNeeded(TextArea textArea, ref VBStatement statement, int lineNr)
		{
			Stack<Token> tokens = new Stack<Token>();
			List<Token> missingEnds = new List<Token>();
			
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(textArea.Document.TextContent));
			
			Token currentToken = null;
			Token prevToken = null;
			
			while ((currentToken = lexer.NextToken()).Kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				
				if (IsBlockStart(lexer, currentToken, prevToken)) {
					if ((tokens.Count > 0 && tokens.Peek().Kind != Tokens.Interface) || IsDeclaration(currentToken.Kind))
						tokens.Push(currentToken);
				}
				
				if (IsBlockEnd(currentToken, prevToken)) {
					while (tokens.Count > 0 && !IsMatchingEnd(tokens.Peek(), currentToken)) {
						missingEnds.Add(tokens.Pop());
					}
					
					if (tokens.Count > 0) {
						if (IsMatchingEnd(tokens.Peek(), currentToken))
							tokens.Pop();
					}
				}
				
				prevToken = currentToken;
			}
			
			while (tokens.Count > 0)
				missingEnds.Add(tokens.Pop());
			
			if (missingEnds.Count > 0)
				return GetClosestMissing(missingEnds, statement, textArea, lineNr) != null;
			else
				return false;
		}
		
		Token GetClosestMissing(List<Token> missingEnds, VBStatement statement, TextArea textArea, int lineNr)
		{
			Token closest = null;
			int diff = 0;
			
			foreach (Token t in missingEnds) {
				if (!IsSingleLine(t.Location.Line, textArea)) {
					if (IsMatchingStatement(t, statement) && ((diff = lineNr - t.Location.Line + 1) > -1)) {
						if (closest == null) {
							closest = t;
						} else {
							if (diff < lineNr - closest.Location.Line + 1)
								closest = t;
						}
					}
				}
			}
			
			return closest;
		}
		
		bool IsSingleLine(int line, TextArea textArea)
		{
			if (line < 1)
				return false;
			
			LineSegment lineSeg = textArea.Document.GetLineSegment(line - 1);
			
			if (lineSeg == null)
				return false;
			
			string text = textArea.Document.GetText(lineSeg);
			
			if (StripComment(text).Trim(' ', '\t', '\r', '\n').EndsWith("_"))
				return true;
			else
				return false;
		}
		
		bool IsMatchingEnd(Token begin, Token end)
		{
			if (begin.Kind == end.Kind)
				return true;
			
			if (begin.Kind == Tokens.For && end.Kind == Tokens.Next)
				return true;
			
			if (begin.Kind == Tokens.Do && end.Kind == Tokens.Loop)
				return true;
			
			return false;
		}
		
		bool IsMatchingStatement(Token token, VBStatement statement)
		{
			if (token.Kind == Tokens.For && statement.EndStatement == "Next")
				return true;
			
			if (token.Kind == Tokens.Do && statement.EndStatement.StartsWith("Loop"))
				return true;
			
			bool empty = !string.IsNullOrEmpty(token.Value);
			bool match = statement.EndStatement.IndexOf(token.Value, StringComparison.InvariantCultureIgnoreCase) != -1;
			
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
			
			Stack<string> indentation = new Stack<string>();
			
			indentation.Push(string.Empty);
			
			int oldLine = 0;
			
			bool inInterface = false;
			bool isMustOverride = false;
			bool isDeclare = false;
			bool isDelegate = false;
			
			Token currentToken = null;
			Token prevToken = null;
			
			while ((currentToken = lexer.NextToken()).Kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				
				if (currentToken.Kind == Tokens.MustOverride)
					isMustOverride = true;
				
				if (currentToken.Kind == Tokens.Delegate)
					isDelegate = true;
				
				if (currentToken.Kind == Tokens.Declare)
					isDeclare = true;
				
				if (currentToken.Kind == Tokens.EOL)
					isDelegate = isDeclare = isMustOverride = false;
				
				if (IsBlockEnd(currentToken, prevToken)) {
					ApplyToRange(textArea, indentation, oldLine, currentToken.Location.Line - 1, begin, end);
					
					if (currentToken.Kind == Tokens.Interface)
						inInterface = false;
					
					if (!inInterface && !isMustOverride && !isDeclare && !isDelegate) {
						Unindent(indentation);
						
						if (currentToken.Kind == Tokens.Select)
							Unindent(indentation);
					}
					
					oldLine = currentToken.Location.Line - 1;
				}
				
				if (IsBlockStart(lexer, currentToken, prevToken)) {
					int line = GetLastVisualLine(currentToken.Location.Line, textArea);
					ApplyToRange(textArea, indentation, oldLine, line, begin, end);
					
					if (!inInterface && !isMustOverride && !isDeclare && !isDelegate) {
						Indent(textArea, indentation);
						
						if (currentToken.Kind == Tokens.Select)
							Indent(textArea, indentation);
					}
					
					if (currentToken.Kind == Tokens.Interface)
						inInterface = true;
					
					oldLine = line;
				}
				
				prevToken = currentToken;
			}
			
			// do last indent step
			int newLine = prevToken.Location.Line;
			
			if (oldLine > newLine)
				newLine = oldLine + 1;
			
			ApplyToRange(textArea, indentation, oldLine, newLine, begin, end);
			
			return (indentation.PeekOrDefault() ?? string.Empty).Length;
		}
		
		int GetLastVisualLine(int line, TextArea area)
		{
			string text = StripComment(area.Document.GetText(area.Document.GetLineSegment(line - 1)));
			while (text.EndsWith("_", StringComparison.Ordinal)) {
				line++;
				text = StripComment(area.Document.GetText(area.Document.GetLineSegment(line - 1)));
			}
			return line;
		}

		void Unindent(Stack<string> indentation)
		{
			indentation.PopOrDefault();
		}

		void Indent(TextArea textArea, Stack<string> indentation)
		{
			bool useSpaces = textArea.TextEditorProperties.ConvertTabsToSpaces;
			int indentationSize = textArea.TextEditorProperties.IndentationSize;
			
			string addIndent = (useSpaces) ? new string(' ', indentationSize) : "\t";
			
			indentation.Push((indentation.PeekOrDefault() ?? string.Empty) + addIndent);
		}
		
		bool IsBlockStart(ILexer lexer, Token current, Token prev)
		{
			if (blockTokens.Contains(current.Kind)) {
				if (current.Kind == Tokens.If) {
					if (prev.Kind != Tokens.EOL)
						return false;
					
					lexer.StartPeek();
					
					Token currentToken = null;
					
					while ((currentToken = lexer.Peek()).Kind != Tokens.EOL) {
						if (currentToken.Kind == Tokens.Then)
							return lexer.Peek().Kind == Tokens.EOL;
					}
				}
				
				if (current.Kind == Tokens.Function) {
					lexer.StartPeek();
					
					if (lexer.Peek().Kind == Tokens.OpenParenthesis)
						return false;
				}
				
				if (current.Kind == Tokens.With && prev.Kind != Tokens.EOL)
					return false;
				
				if (current.Kind == Tokens.While && (prev.Kind == Tokens.Skip || prev.Kind == Tokens.Take))
					return false;
				
				if (current.Kind == Tokens.Select && prev.Kind != Tokens.EOL)
					return false;
				
				if (current.Kind == Tokens.Class || current.Kind == Tokens.Structure) {
					lexer.StartPeek();
					
					Token t = lexer.Peek();
					
					if (t.Kind == Tokens.CloseParenthesis || t.Kind == Tokens.CloseCurlyBrace || t.Kind == Tokens.Comma)
						return false;
				}
				
				if (current.Kind == Tokens.Module) {
					lexer.StartPeek();
					
					Token t = lexer.Peek();
					
					if (t.Kind == Tokens.Colon)
						return false;
				}
				
				if (prev.Kind == Tokens.End ||
				    prev.Kind == Tokens.Loop ||
				    prev.Kind == Tokens.Exit ||
				    prev.Kind == Tokens.Continue ||
				    prev.Kind == Tokens.Resume ||
				    prev.Kind == Tokens.GoTo ||
				    prev.Kind == Tokens.Do)
					return false;
				else
					return true;
			}
			
			return IsSpecialCase(current, prev);
		}
		
		bool IsBlockEnd(Token current, Token prev)
		{
			if (current.Kind == Tokens.Next) {
				if (prev.Kind == Tokens.Resume)
					return false;
				else
					return true;
			}
			
			if (current.Kind == Tokens.Loop)
				return true;
			
			if (blockTokens.Contains(current.Kind)) {
				if (prev.Kind == Tokens.End)
					return true;
				else
					return false;
			}
			
			return IsSpecialCase(current, prev);
		}
		
		bool IsSpecialCase(Token current, Token prev)
		{
			switch (current.Kind) {
				case Tokens.Else:
					return true;
				case Tokens.Case:
					return prev.Kind != Tokens.Select;
				case Tokens.ElseIf:
					return true;
				case Tokens.Catch:
					return true;
				case Tokens.Finally:
					return true;
			}
			
			return false;
		}
		
		void ApplyToRange(TextArea textArea, Stack<string> indentation, int begin, int end, int selBegin, int selEnd)
		{
			bool multiLine = false;
			
			for (int i = begin; i < end; i++) {
				LineSegment curLine = textArea.Document.GetLineSegment(i);
				string lineText = textArea.Document.GetText(curLine).Trim(' ', '\t', '\r', '\n');
				string noComments = StripComment(lineText).TrimEnd(' ', '\t', '\r', '\n');
				
				if (i < selBegin || i > selEnd) {
					indentation.PopOrDefault();
					indentation.Push(GetIndentation(textArea, i));
				}
				
				// change indentation before (indent this line)
				if (multiLine && noComments.EndsWith("}")) {
					Unindent(indentation);
					multiLine = false;
				}
				
				SmartReplaceLine(textArea.Document, curLine, (indentation.PeekOrDefault() ?? string.Empty) + lineText);
				
				// change indentation afterwards (indent next line)
				if (!multiLine && noComments.EndsWith("_")) {
					Indent(textArea, indentation);
					multiLine = true;
				}

				if (multiLine && !noComments.EndsWith("_")) {
					multiLine = false;
					Unindent(indentation);
				}
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
