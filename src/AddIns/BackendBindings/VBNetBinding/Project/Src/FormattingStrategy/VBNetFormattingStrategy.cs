// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.VBNetBinding
{
	/// <summary>
	/// This class handles the auto and smart indenting in the textbuffer while
	/// you type.
	/// </summary>
	public class VBNetFormattingStrategy : DefaultFormattingStrategy
	{
		#region VB Statements
		static readonly List<VBStatement> statements;
		
		internal static List<VBStatement> Statements {
			get { return statements; }
		}
		
		static readonly string[] keywords = new string[] {
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
		
		internal static IList<string> Keywords {
			get { return keywords; }
		}
		
		static VBStatement interfaceStatement;
		
		static List<int> blockTokens = new List<int>(
			new int[] {
				Tokens.Class, Tokens.Module, Tokens.Namespace, Tokens.Interface, Tokens.Structure,
				Tokens.Sub, Tokens.Function, Tokens.Operator, Tokens.Enum,
				Tokens.If, Tokens.For, Tokens.Do, Tokens.While, Tokens.With, Tokens.Select, Tokens.Try, Tokens.Using, Tokens.SyncLock,
				Tokens.Property, Tokens.Get, Tokens.Set
			});
		#endregion
		
		bool doCasing;
		bool doInsertion;
		
		static VBNetFormattingStrategy()
		{
			statements = new List<VBStatement>();
			statements.Add(new VBStatement(@"^if.*?(then|\s+_)$", "^end ?if$", "End If", 1, Tokens.If));
			statements.Add(new VBStatement(@"\bclass\s+\w+\s*($|\(\s*Of)", "^end class$", "End Class", 1, Tokens.Class));
			statements.Add(new VBStatement(@"\bnamespace\s+\w+(\.\w+)*$", "^end namespace$", "End Namespace", 1, Tokens.Namespace));
			statements.Add(new VBStatement(@"\bmodule\s+\w+$", "^end module$", "End Module", 1, Tokens.Module));
			statements.Add(new VBStatement(@"\bstructure\s+\w+\s*($|\(\s*Of)", "^end structure$", "End Structure", 1, Tokens.Structure));
			statements.Add(new VBStatement(@"^while\s+", "^end while$", "End While", 1, Tokens.While));
			statements.Add(new VBStatement(@"^select case", "^end select$", "End Select", 1, Tokens.Select));
			statements.Add(new VBStatement(@"(?<!\b(delegate|mustoverride|declare(\s+(unicode|ansi|auto))?)\s+)\bsub\s+\w+", @"^end\s+sub$", "End Sub", 1, Tokens.Sub));
			statements.Add(new VBStatement(@"(?<!\bmustoverride (readonly |writeonly )?)\bproperty\s+\w+", @"^end\s+property$", "End Property", 1, Tokens.Property));
			statements.Add(new VBStatement(@"(?<!\b(delegate|mustoverride|declare(\s+(unicode|ansi|auto))?)\s+)\bfunction\s+\w+", @"^end\s+function$", "End Function", 1, Tokens.Function));
			statements.Add(new VBStatement(@"\boperator(\s*[\+\-\*\/\&\^\>\<\=\\]+\s*|\s+\w+\s*)\(", @"^end\s+operator$", "End Operator", 1, Tokens.Operator));
			statements.Add(new VBStatement(@"\bfor\s+.*?$", "^next( \\w+)?$", "Next", 1, Tokens.For));
			statements.Add(new VBStatement(@"^synclock\s+.*?$", "^end synclock$", "End SyncLock", 1, Tokens.SyncLock));
			statements.Add(new VBStatement(@"^get$", "^end get$", "End Get", 1, Tokens.Get));
			statements.Add(new VBStatement(@"^with\s+.*?$", "^end with$", "End With", 1, Tokens.With));
			statements.Add(new VBStatement(@"^set(\s*\(.*?\))?$", "^end set$", "End Set", 1, Tokens.Set));
			statements.Add(new VBStatement(@"^try$", "^end try$", "End Try", 1, Tokens.Try));
			statements.Add(new VBStatement(@"^do\s+.+?$", "^loop$", "Loop", 1, Tokens.Do));
			statements.Add(new VBStatement(@"^do$", "^loop .+?$", "Loop While ", 1, Tokens.Do));
			statements.Add(new VBStatement(@"\benum\s+\w+$", "^end enum$", "End Enum", 1, Tokens.Enum));
			interfaceStatement = new VBStatement(@"\binterface\s+\w+\s*($|\(\s*Of)", "^end interface$", "End Interface", 1, Tokens.Interface);
			statements.Add(interfaceStatement);
			statements.Add(new VBStatement(@"\busing\s+", "^end using$", "End Using", 1, Tokens.Using));
			statements.Add(new VBStatement(@"^#region\s+", "^#end region$", "#End Region", 0, -1));
		}
		
		public override void FormatLine(ITextEditor editor, char charTyped)
		{
			using (editor.Document.OpenUndoGroup()) {
				FormatLineInternal(editor, editor.Caret.Line, editor.Caret.Offset, charTyped);
			}
		}
		
		void FormatLineInternal(ITextEditor editor, int lineNr, int cursorOffset, char ch)
		{
			string terminator = DocumentUtilitites.GetLineTerminator(editor.Document, lineNr);
			doCasing = PropertyService.Get("VBBinding.TextEditor.EnableCasing", true);
			doInsertion = PropertyService.Get("VBBinding.TextEditor.EnableEndConstructs", true);
			
			IDocumentLine currentLine = editor.Document.GetLine(lineNr);
			IDocumentLine lineAbove = lineNr > 1 ? editor.Document.GetLine(lineNr - 1) : null;
			
			string curLineText = currentLine == null ? "" : currentLine.Text;
			string lineAboveText = lineAbove == null ? "" : lineAbove.Text;
			
			if (ch == '\'') {
				InsertDocumentationComments(editor, lineNr, cursorOffset);
			}
			
			if (ch == '\n' && lineAbove != null) {
				if (LanguageUtils.IsInsideDocumentationComment(editor, lineAbove, lineAbove.EndOffset)) {
					editor.Document.Insert(cursorOffset, "''' ");
					return;
				}
				
				string textToReplace = lineAboveText.TrimLine();
				
				if (doCasing)
					DoCasingOnLine(lineAbove, textToReplace, editor);
				
				if (doInsertion)
					DoInsertionOnLine(terminator, currentLine, lineAbove, textToReplace, editor, lineNr);
				
				if (IsInString(lineAboveText)) {
					if (IsFinishedString(curLineText)) {
						editor.Document.Insert(lineAbove.EndOffset, "\" & _");
						editor.Document.Insert(currentLine.Offset, "\"");
					} else {
						editor.Document.Insert(lineAbove.EndOffset, "\"");
					}
				} else {
					string indent = DocumentUtilitites.GetWhitespaceAfter(editor.Document, lineAbove.Offset);
					if (indent.Length > 0) {
						string newLineText = indent + currentLine.Text.Trim();
						editor.Document.Replace(currentLine.Offset, currentLine.Length, newLineText);
					}
					editor.Caret.Column = indent.Length + 1;
				}
				
				IndentLines(editor, lineNr - 1, lineNr);
			} else if(ch == '>') {
				if (LanguageUtils.IsInsideDocumentationComment(editor, currentLine, cursorOffset)) {
					int column = editor.Caret.Offset - currentLine.Offset;
					int index = Math.Min(column - 1, curLineText.Length - 1);
					
					while (index > 0 && curLineText[index] != '<') {
						--index;
						if(curLineText[index] == '/')
							return; // the tag was an end tag or already
					}
					
					if (index > 0) {
						StringBuilder commentBuilder = new StringBuilder("");
						for (int i = index; i < curLineText.Length && i < column && !Char.IsWhiteSpace(curLineText[i]); ++i) {
							commentBuilder.Append(curLineText[i]);
						}
						string tag = commentBuilder.ToString().Trim();
						if (!tag.EndsWith(">", StringComparison.OrdinalIgnoreCase)) {
							tag += ">";
						}
						if (!tag.StartsWith("/", StringComparison.OrdinalIgnoreCase)) {
							string endTag = "</" + tag.Substring(1);
							editor.Document.Insert(editor.Caret.Offset, endTag);
							editor.Caret.Offset -= endTag.Length;
						}
					}
				}
			}
		}

		void DoInsertionOnLine(string terminator, IDocumentLine currentLine, IDocumentLine lineAbove, string textToReplace, ITextEditor editor, int lineNr)
		{
			string curLineText = currentLine.Text;
			
			if (Regex.IsMatch(textToReplace.Trim(), "^If .*[^_]$", RegexOptions.IgnoreCase)) {
				if (!Regex.IsMatch(textToReplace, "\\bthen\\b", RegexOptions.IgnoreCase)) {
					string specialThen = "Then"; // do special check in cases like If t = True' comment
					if (editor.Document.GetCharAt(lineAbove.Offset + textToReplace.Length) == '\'')
						specialThen += " ";
					if (editor.Document.GetCharAt(lineAbove.Offset + textToReplace.Length - 1) != ' ')
						specialThen = " " + specialThen;
					editor.Document.Insert(lineAbove.Offset + textToReplace.Length, specialThen);
					textToReplace += specialThen;
				}
			}
			
			// check #Region statements
			if (Regex.IsMatch(textToReplace.Trim(), "^#Region", RegexOptions.IgnoreCase) && LookForEndRegion(editor)) {
				string indentation = DocumentUtilitites.GetWhitespaceAfter(editor.Document, lineAbove.Offset);
				textToReplace += indentation + "\r\n" + indentation + "#End Region";
				editor.Document.Replace(currentLine.Offset, currentLine.Length, textToReplace);
			}
			
			foreach (VBStatement statement_ in statements) {
				VBStatement statement = statement_;	// allow passing statement byref
				if (Regex.IsMatch(textToReplace.Trim(), statement.StartRegex, RegexOptions.IgnoreCase)) {
					string indentation = DocumentUtilitites.GetWhitespaceAfter(editor.Document, lineAbove.Offset);
					if (IsEndStatementNeeded(editor, ref statement, lineNr)) {
						editor.Document.Replace(currentLine.Offset, currentLine.Length, terminator + indentation + statement.EndStatement);
					}
					if (!IsInsideInterface(editor, lineNr) || statement == interfaceStatement) {
						for (int i = 0; i < statement.IndentPlus; i++) {
							indentation += editor.Options.IndentationString;
						}
					}
					editor.Document.Replace(currentLine.Offset, currentLine.Length, indentation + curLineText.Trim());
					editor.Caret.Line = currentLine.LineNumber;
					editor.Caret.Column = indentation.Length;
					return;
				}
			}
		}
		
		static void DoCasingOnLine(IDocumentLine lineAbove, string textToReplace, ITextEditor editor)
		{
			foreach (string keyword in keywords) {
				string regex = "\\b" + keyword + "\\b"; // \b = word border
				MatchCollection matches = Regex.Matches(textToReplace, regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
				foreach (Match match in matches) {
					if (keyword == "EndIf") // special case
						editor.Document.Replace(lineAbove.Offset + match.Index, match.Length, "End If");
					else
						editor.Document.Replace(lineAbove.Offset + match.Index, match.Length, keyword);
				}
			}
		}

		static void InsertDocumentationComments(ITextEditor editor, int lineNr, int cursorOffset)
		{
			string terminator = DocumentUtilitites.GetLineTerminator(editor.Document, lineNr);
			
			IDocumentLine currentLine = editor.Document.GetLine(lineNr);
			IDocumentLine previousLine = (lineNr > 1) ? editor.Document.GetLine(lineNr - 1) : null;
			
			string curLineText = currentLine.Text;
			string lineAboveText = previousLine == null ? null : previousLine.Text;
			
			if (curLineText != null && curLineText.EndsWith("'''", StringComparison.OrdinalIgnoreCase) && (lineAboveText == null || !lineAboveText.Trim().StartsWith("'''", StringComparison.OrdinalIgnoreCase))) {
				string indentation = DocumentUtilitites.GetWhitespaceAfter(editor.Document, currentLine.Offset);
				object member = GetMemberAfter(editor, lineNr);
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
					editor.Document.Insert(cursorOffset, sb.ToString());
					editor.Caret.Position = editor.Document.OffsetToPosition(cursorOffset + indentation.Length + "/// ".Length + " <summary>".Length + terminator.Length);
				}
			}
		}
		
		static bool LookForEndRegion(ITextEditor editor)
		{
			string lineText = editor.Document.GetLine(1).Text;
			int count = 0;
			int lineNr = 0;
			while ((!Regex.IsMatch(lineText, @"^\s*#End\s+Region", RegexOptions.IgnoreCase) || count >= 0) && (lineNr < editor.Document.TotalNumberOfLines)) {
				if (Regex.IsMatch(lineText, @"^\s*#Region", RegexOptions.IgnoreCase))
					count++;
				if (Regex.IsMatch(lineText, @"^\s*#End\s+Region", RegexOptions.IgnoreCase))
					count--;
				lineNr++;
				if (lineNr < editor.Document.TotalNumberOfLines)
					lineText = editor.Document.GetLine(1).Text;
			}
			
			return (count > 0);
		}
		
		static bool IsInsideInterface(ITextEditor editor, int lineNr)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(editor.Document.Text));
			
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
				
				prevToken = currentToken;
			}
			
			if (tokens.Count > 0)
				return tokens.Pop().Kind == Tokens.Interface;
			
			return false;
		}
		
		static bool IsInString(string start)
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
		
		static bool IsFinishedString(string end)
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
		
		internal static bool IsDeclaration(int type)
		{
			return (type == Tokens.Class) ||
				(type == Tokens.Module) ||
				(type == Tokens.Structure) ||
				(type == Tokens.Enum) ||
				(type == Tokens.Interface);
		}
		
		bool IsEndStatementNeeded(ITextEditor editor, ref VBStatement statement, int lineNr)
		{
			Stack<Token> tokens = new Stack<Token>();
			List<Token> missingEnds = new List<Token>();
			
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(editor.Document.Text));
			
			Token currentToken = null;
			Token prevToken = null;
			
			while ((currentToken = lexer.NextToken()).Kind != Tokens.EOF) {
				if (prevToken == null)
					prevToken = currentToken;
				if (IsBlockStart(lexer, currentToken, prevToken) && !IsAutomaticPropertyWithDefaultValue(lexer, currentToken, prevToken)) {
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
				return GetClosestMissing(missingEnds, statement, editor, lineNr) != null;
			else
				return false;
		}
		
		static bool IsAutomaticPropertyWithDefaultValue(ILexer lexer, Token currentToken, Token prevToken)
		{
			if (currentToken.Kind != Tokens.Property)
				return false;
			lexer.StartPeek();
			
			int parenthesesNesting = 0;
			
			// look for parameter list, = or EOL
			Token t;
			while ((t = lexer.Peek()).Kind != Tokens.EOF) {
				if (t.Kind == Tokens.OpenParenthesis)
					parenthesesNesting++;
				if (t.Kind == Tokens.CloseParenthesis)
					parenthesesNesting--;
				if (parenthesesNesting == 0 && t.Kind == Tokens.Assign)
					return true;
				if (t.Kind == Tokens.EOL)
					return false;
			}
			
			return false;
		}
		
		static Token GetClosestMissing(List<Token> missingEnds, VBStatement statement, ITextEditor editor, int lineNr)
		{
			Token closest = null;
			int diff = 0;
			
			foreach (Token t in missingEnds) {
				if (!IsSingleLine(t.Location.Line, editor)) {
					if (IsMatchingStatement(t, statement) && ((diff = lineNr - t.Location.Line) > 0)) {
						if (closest == null) {
							closest = t;
						} else {
							if (diff < lineNr - closest.Location.Line)
								closest = t;
						}
					}
				}
			}
			
			return closest;
		}
		
		static bool IsSingleLine(int line, ITextEditor editor)
		{
			if (line < 1)
				return false;
			
			IDocumentLine lineSeg = editor.Document.GetLine(line);
			
			if (lineSeg == null)
				return false;
			
			string text = lineSeg.Text;
			
			if (text.TrimComments().Trim(' ', '\t', '\r', '\n').EndsWith("_", StringComparison.OrdinalIgnoreCase))
				return true;
			else
				return false;
		}
		
		internal static bool IsMatchingEnd(Token begin, Token end)
		{
			if (begin.Kind == end.Kind)
				return true;
			
			if (begin.Kind == Tokens.For && end.Kind == Tokens.Next)
				return true;
			
			if (begin.Kind == Tokens.Do && end.Kind == Tokens.Loop)
				return true;
			
			return false;
		}
		
		static bool IsMatchingStatement(Token token, VBStatement statement)
		{
			if (token.Kind == Tokens.For && statement.EndStatement == "Next")
				return true;
			
			if (token.Kind == Tokens.Do && statement.EndStatement.StartsWith("Loop", StringComparison.OrdinalIgnoreCase))
				return true;
			
			bool empty = !string.IsNullOrEmpty(token.Value);
			bool match = statement.EndStatement.IndexOf(token.Value, StringComparison.OrdinalIgnoreCase) != -1;
			
			return empty && match;
		}
		
		public override void IndentLines(ITextEditor editor, int begin, int end)
		{
			SmartIndentInternal(editor, begin, end);
		}
		
		static int SmartIndentInternal(ITextEditor editor, int begin, int end)
		{
			ILexer lexer = ParserFactory.CreateLexer(SupportedLanguage.VBNet, new StringReader(editor.Document.Text));
			
			ExpressionFinder context = new ExpressionFinder();
			
			Stack<string> indentation = new Stack<string>();
			indentation.Push(string.Empty);
			
			List<int> eols = new List<int>();
			
			bool inInterface = false;
			bool isMustOverride = false;
			bool isDeclare = false;
			bool isDelegate = false;
			
			Token currentToken = null;
			Token prevToken = null;
			
			int blockStart = 1;
			int lambdaNesting = 0;
			
			bool sawAttribute = false;
			
			while ((currentToken = lexer.NextToken()).Kind != Tokens.EOF) {
				if (context.InContext(Context.Attribute) && currentToken.Kind == Tokens.GreaterThan)
					sawAttribute = true;
				
				context.InformToken(currentToken);
				
				if (prevToken == null)
					prevToken = currentToken;
				
				if (currentToken.Kind == Tokens.MustOverride)
					isMustOverride = true;
				
				if (currentToken.Kind == Tokens.Delegate)
					isDelegate = true;
				
				if (currentToken.Kind == Tokens.Declare)
					isDeclare = true;
				
				if (currentToken.Kind == Tokens.EOL) {
					isDelegate = isDeclare = isMustOverride = sawAttribute = false;
					eols.Add(currentToken.Location.Line);
				}
				
				if (IsBlockEnd(currentToken, prevToken)) {
					// indent the lines inside the block
					// this is an End-statement
					// hence we indent from blockStart to the previous line
					int blockEnd = currentToken.Location.Line - 1;
					
					// if this is a lambda end include End-Statement in block
//					if (lambdaNesting > 0 && (currentToken.Kind == Tokens.Function || currentToken.Kind == Tokens.Sub)) {
//						blockEnd++;
//					}
					
					ApplyToRange(editor, indentation, eols, blockStart, blockEnd, begin, end, sawAttribute);
					
					if (lambdaNesting > 0 && (currentToken.Kind == Tokens.Function || currentToken.Kind == Tokens.Sub)) {
						Unindent(indentation);
						
						ApplyToRange(editor, indentation, eols, currentToken.Location.Line, currentToken.Location.Line, begin, end, sawAttribute);
					}
					
					if (currentToken.Kind == Tokens.Interface)
						inInterface = false;
					
					if (!inInterface && !isMustOverride && !isDeclare && !isDelegate) {
						Unindent(indentation);
						
						if (currentToken.Kind == Tokens.Select)
							Unindent(indentation);
					}
					
					// block start is this line (for the lines between two blocks)
					blockStart = currentToken.Location.Line;
					
					if (lambdaNesting > 0 && (currentToken.Kind == Tokens.Function || currentToken.Kind == Tokens.Sub)) {
						blockStart++;
						lambdaNesting--;
					}
				}
				
				bool isMultiLineLambda;
				if (IsBlockStart(lexer, currentToken, prevToken, out isMultiLineLambda)) {
					// indent the lines between the last and this block
					// this is a Begin-statement
					// hence we indent from blockStart to the this line
					int lastVisualLine = FindNextEol(lexer);
					eols.Add(lastVisualLine);
					ApplyToRange(editor, indentation, eols, blockStart, lastVisualLine, begin, end, sawAttribute);
					
					if (isMultiLineLambda && (currentToken.Kind == Tokens.Function || currentToken.Kind == Tokens.Sub)) {
						lambdaNesting++;
						int endColumn = currentToken.Location.Column;
						if (prevToken.Kind == Tokens.Iterator || prevToken.Kind == Tokens.Async)
							endColumn = prevToken.Location.Column;
						int startColumn = DocumentUtilitites.GetWhitespaceAfter(editor.Document, editor.Document.GetLine(lastVisualLine).Offset).Length;
						if (startColumn < endColumn)
							Indent(editor, indentation, new string(' ', endColumn - startColumn - 1));
					}
					
					if (!inInterface && !isMustOverride && !isDeclare && !isDelegate && !IsAutomaticPropertyWithDefaultValue(lexer, currentToken, prevToken)) {
						Indent(editor, indentation);
						
						if (currentToken.Kind == Tokens.Select)
							Indent(editor, indentation);
					}
					
					if (currentToken.Kind == Tokens.Interface)
						inInterface = true;
					
					// block start is the following line (for the lines inside a block)
					blockStart = lastVisualLine + 1;
				}
				
				prevToken = currentToken;
			}
			
			ApplyToRange(editor, indentation, eols, blockStart, editor.Document.TotalNumberOfLines, begin, end, sawAttribute);
			
			return (indentation.PeekOrDefault() ?? string.Empty).Length;
		}
		
		static int FindNextEol(ILexer lexer)
		{
			lexer.StartPeek();
			
			Token t = lexer.Peek();
			
			while (t.Kind > Tokens.EOL) // break on EOF(0) or EOL(1)
				t = lexer.Peek();
			
			return t.Location.Line;
		}
		
		static void ApplyToRange(ITextEditor editor, Stack<string> indentation, List<int> eols, int blockStart, int blockEnd, int selectionStart, int selectionEnd, bool sawAttribute) {
			LoggingService.InfoFormatted("indenting line {0} to {1} with {2}", blockStart, blockEnd, (indentation.PeekOrDefault() ?? "").Length);
			
			int nextEol = -1;
			bool wasMultiLine = false;
			
			for (int i = blockStart; i <= blockEnd; i++) {
				IDocumentLine curLine = editor.Document.GetLine(i);
				string lineText = curLine.Text.TrimStart();
				// preprocessor directives cannot be multiline (just as comments)
				// and they are not included in normal block indentation ->
				// treat preprocessor directives as comments -> remove them
				string noComments = lineText.TrimComments().TrimPreprocessorDirectives().TrimEnd().TrimEnd('_').TrimEnd();
				
				// adjust indentation if the current line is not selected
				// lines between the selection will be aligned to the selected level
				if (i < selectionStart || i > selectionEnd) {
					indentation.PopOrDefault();
					indentation.Push(DocumentUtilitites.GetWhitespaceAfter(editor.Document, curLine.Offset));
				}
				
				// look for next eol if line is not empty
				// (the lexer does not produce eols for empty lines)
				if (!string.IsNullOrEmpty(noComments) && i >= nextEol) {
					int search = eols.BinarySearch(i);
					if (search < 0)
						search = ~search;
					nextEol = search < eols.Count ? eols[search] : i;
				}
				
				// remove indentation in last line of multiline array(, collection, object) initializers
				if (i == nextEol && wasMultiLine && (noComments == "}" || sawAttribute)) {
					wasMultiLine = false;
					Unindent(indentation);
				}
				
				// apply the indentation
				editor.Document.SmartReplaceLine(curLine, (indentation.PeekOrDefault() ?? "") + lineText);
				
				// indent line if it is ended by (implicit) line continuation
				if (i < nextEol && !wasMultiLine) {
					wasMultiLine = true;
					Indent(editor, indentation);
				}
				
				// unindent if this is the last line of a multiline statement
				if (i == nextEol && wasMultiLine) {
					wasMultiLine = false;
					Unindent(indentation);
				}
			}
		}

		static void Unindent(Stack<string> indentation)
		{
			indentation.PopOrDefault();
		}

		static void Indent(ITextEditor editor, Stack<string> indentation, string indent = null)
		{
			indentation.Push((indentation.PeekOrDefault() ?? string.Empty) + (indent ?? editor.Options.IndentationString));
		}
		
		internal static bool IsBlockStart(ILexer lexer, Token current, Token prev)
		{
			bool tmp;
			return IsBlockStart(lexer, current, prev, out tmp);
		}
		
		static bool IsBlockStart(ILexer lexer, Token current, Token prev, out bool isMultiLineLambda)
		{
			isMultiLineLambda = false;
			
			if (blockTokens.Contains(current.Kind)) {
				if (current.Kind == Tokens.If) {
					if (prev.Kind != Tokens.EOL)
						return false;
					
					lexer.StartPeek();
					
					Token currentToken = null;
					
					while ((currentToken = lexer.Peek()).Kind > Tokens.EOL) {
						if (currentToken.Kind == Tokens.Then)
							return lexer.Peek().Kind == Tokens.EOL;
					}
				}
				
				// check if it is a lambda
				if (current.Kind == Tokens.Function || current.Kind == Tokens.Sub) {
					lexer.StartPeek();
					
					bool isSingleLineLambda = false;
					
					if (lexer.Peek().Kind == Tokens.OpenParenthesis) {
						isSingleLineLambda = true;
						
						int brackets = 1;
						
						// look for end of parameter list
						while (brackets > 0) {
							var t = lexer.Peek();
							if (t.Kind == Tokens.OpenParenthesis)
								brackets++;
							if (t.Kind == Tokens.CloseParenthesis)
								brackets--;
						}
						
						// expression is multi-line lambda if next Token is EOL
						if (brackets == 0)
							return isMultiLineLambda = (lexer.Peek().Kind == Tokens.EOL);
					}
					
					// do not indent if current token is start of single-line lambda
					if (isSingleLineLambda)
						return false;
				}
				
				if (current.Kind == Tokens.With && prev.Kind > Tokens.EOL)
					return false;
				
				if (current.Kind == Tokens.While && (prev.Kind == Tokens.Skip || prev.Kind == Tokens.Take))
					return false;
				
				if (current.Kind == Tokens.Select && prev.Kind > Tokens.EOL)
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
		
		internal static bool IsBlockEnd(Token current, Token prev)
		{
			if (current.Kind == Tokens.Next)
				return prev.Kind == Tokens.EOL || prev.Kind == Tokens.Colon;
			
			if (current.Kind == Tokens.Loop)
				return prev.Kind == Tokens.EOL || prev.Kind == Tokens.Colon;
			
			if (blockTokens.Contains(current.Kind))
				return prev.Kind == Tokens.End;
			
			return IsSpecialCase(current, prev);
		}
		
		static bool IsSpecialCase(Token current, Token prev)
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
		
		/// <summary>
		/// Gets the next member after the specified caret position.
		/// </summary>
		static object GetMemberAfter(ITextEditor editor, int caretLine)
		{
			string fileName = editor.FileName;
			object nextElement = null;
			if (fileName != null && fileName.Length > 0 ) {
				ParseInformation parseInfo = ParserService.ParseFile(fileName, editor.Document);
				if (parseInfo != null) {
					ICompilationUnit currentCompilationUnit = parseInfo.CompilationUnit;
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
							foreach (IMember m in currentClass.AllMembers) {
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
		
		public override void IndentLine(ITextEditor editor, IDocumentLine line)
		{
			IndentLines(editor, line.LineNumber, line.LineNumber);
		}
		
		public override void SurroundSelectionWithComment(ITextEditor editor)
		{
			SurroundSelectionWithSingleLineComment(editor, "'");
		}
	}
}
