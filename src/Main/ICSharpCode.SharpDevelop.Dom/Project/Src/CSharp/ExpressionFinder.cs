// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1751 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.CSharp;

namespace ICSharpCode.SharpDevelop.Dom.CSharp
{
	/// <summary>
	/// Supports getting the expression including context from the cursor position.
	/// </summary>
	public class CSharpExpressionFinder : IExpressionFinder
	{
		ParseInformation parseInformation;
		IProjectContent projectContent;
		
		public CSharpExpressionFinder(ParseInformation parseInformation)
		{
			this.parseInformation = parseInformation;
			if (parseInformation != null && parseInformation.MostRecentCompilationUnit != null) {
				projectContent = parseInformation.MostRecentCompilationUnit.ProjectContent;
			} else {
				projectContent = DefaultProjectContent.DummyProjectContent;
			}
		}
		
		ILexer lexer;
		Location targetPosition;
		List<int> lineOffsets;
		
		int LocationToOffset(Location location)
		{
			if (location.Line <= 0) return -1;
			return lineOffsets[location.Line - 1] + location.Column - 1;
		}
		
		enum FrameType
		{
			Global,
			TypeDecl,
			Interface,
			Enum,
			ParameterList,
			Property,
			Event,
			Statements,
			Popped
		}
		
		/// <summary>
		/// Used to support a frame-type specific state machine. Used in TrackCurrentContext
		/// </summary>
		enum FrameState
		{
			/// <summary>
			/// the default state (all frame types)
			/// </summary>
			Normal,
			/// <summary>
			/// parsing an inheritance list (Global+TypeDecl)
			/// </summary>
			InheritanceList,
			/// <summary>
			/// parsing an event declaration (Interface+TypeDecl)
			/// </summary>
			EventDecl,
			/// <summary>
			/// parsing a field declaration (Interface+TypeDecl).
			/// Could also be a property declaration
			/// </summary>
			FieldDecl,
			/// <summary>
			/// parsing a method delcaration (Interface+TypeDecl)
			/// </summary>
			MethodDecl,
			/// <summary>
			/// parsing a field initializer (TypeDecl)
			/// </summary>
			Initializer,
		}
		
		/// <summary>
		/// When parsing the code, each block starting with one of the brackets "(", "[", "{" or "&lt;" (for generics)
		/// gets an instance of Frame.
		/// </summary>
		sealed class Frame
		{
			internal Frame parent;
			internal FrameType type;
			internal FrameType childType;
			internal FrameState state;
			internal char bracketType;
			internal ExpressionContext context;
			internal IReturnType expectedType;
			
			internal void SetContext(ExpressionContext context)
			{
				this.context = context;
				this.expectedType = null;
			}
			internal void SetExpectedType(IReturnType expectedType)
			{
				this.expectedType = expectedType;
				this.context = ExpressionContext.Default;
			}
			internal void SetDefaultContext()
			{
				if (state == FrameState.InheritanceList) {
					if (childType == FrameType.Enum) {
						SetContext(ExpressionContext.EnumBaseType);
					} else if (childType == FrameType.Interface) {
						SetContext(ExpressionContext.Interface);
					} else {
						SetContext(ExpressionContext.InheritableType);
					}
				} else {
					switch (type) {
						case FrameType.Global:
							SetContext(ExpressionContext.Global);
							break;
						case FrameType.TypeDecl:
							SetContext(ExpressionContext.TypeDeclaration);
							break;
						case FrameType.Enum:
							SetContext(ExpressionContext.IdentifierExpected);
							break;
						case FrameType.Interface:
							SetContext(ExpressionContext.InterfaceDeclaration);
							break;
						case FrameType.Event:
							SetContext(ExpressionContext.EventDeclaration);
							break;
						case FrameType.Property:
							if (parent != null && parent.type == FrameType.Interface) {
								SetContext(ExpressionContext.InterfacePropertyDeclaration);
							} else {
								SetContext(ExpressionContext.PropertyDeclaration);
							}
							break;
						case FrameType.Statements:
							SetContext(ExpressionContext.StatementStart);
							break;
						case FrameType.ParameterList:
							SetContext(ExpressionContext.ParameterType);
							break;
						default:
							SetContext(ExpressionContext.Default);
							break;
					}
				}
			}
			
			internal Location lastExpressionStart;
			
			public Frame(Frame parent)
			{
				this.parent = parent;
				if (parent != null) {
					this.type = parent.childType;
					switch (this.type) {
						case FrameType.Property:
						case FrameType.Event:
							this.childType = FrameType.Statements;
							break;
						default:
							this.childType = this.type;
							break;
					}
				}
				SetDefaultContext();
			}
		}
		
		void Init(string text, int offset)
		{
			if (offset < 0 || offset > text.Length)
				throw new ArgumentOutOfRangeException("offset", offset, "offset must be between 0 and " + text.Length);
			lexer = ParserFactory.CreateLexer(SupportedLanguage.CSharp, new StringReader(text));
			lexer.SkipAllComments = true;
			lineOffsets = new List<int>();
			lineOffsets.Add(0);
			for (int i = 0; i < text.Length; i++) {
				if (i == offset) {
					targetPosition = new Location(offset - lineOffsets[lineOffsets.Count - 1] + 1, lineOffsets.Count);
				}
				if (text[i] == '\n') {
					lineOffsets.Add(i + 1);
				}
			}
			if (offset == text.Length) {
				targetPosition = new Location(offset - lineOffsets[lineOffsets.Count - 1] + 1, lineOffsets.Count);
			}
			
			frame = new Frame(null);
			lastToken = Tokens.EOF;
		}
		
		Frame frame;
		int lastToken;
		
		public ExpressionResult FindExpression(string text, int offset)
		{
			offset++;
			Init(text, offset);
			Token token;
			while ((token = lexer.NextToken()) != null) {
				if (token.kind == Tokens.EOF) break;
				
				if (targetPosition < token.EndLocation) {
					break;
				}
				ApplyToken(token);
				lastToken = token.kind;
			}
			
			int tokenOffset;
			if (token == null || token.kind == Tokens.EOF)
				tokenOffset = text.Length;
			else
				tokenOffset = LocationToOffset(token.Location);
			int lastExpressionStartOffset = LocationToOffset(frame.lastExpressionStart);
			if (lastExpressionStartOffset >= 0) {
				if (offset < tokenOffset) {
					// offset is in front of this token
					return new ExpressionResult(text.Substring(lastExpressionStartOffset, tokenOffset - lastExpressionStartOffset), frame.context);
				} else {
					// offset is IN this token
					return new ExpressionResult(text.Substring(lastExpressionStartOffset, offset - lastExpressionStartOffset), frame.context);
				}
			} else {
				return new ExpressionResult(null, frame.context);
			}
		}
		
		void ApplyToken(Token token)
		{
			TrackCurrentFrameAndExpression(token);
			TrackCurrentContext(token);
		}
		
		void TrackCurrentFrameAndExpression(Token token)
		{
			while (frame.bracketType == '<' && !Tokens.ValidInsideTypeName[token.kind]) {
				frame.type = FrameType.Popped;
				frame = frame.parent;
			}
			switch (token.kind) {
				case Tokens.OpenCurlyBrace:
					frame.lastExpressionStart = Location.Empty;
					frame = new Frame(frame);
					frame.parent.childType = frame.parent.type;
					frame.bracketType = '{';
					break;
				case Tokens.CloseCurlyBrace:
					while (frame.parent != null) {
						if (frame.bracketType == '{') {
							frame.type = FrameType.Popped;
							frame = frame.parent;
							break;
						} else {
							frame.type = FrameType.Popped;
							frame = frame.parent;
						}
					}
					break;
				case Tokens.OpenParenthesis:
				case Tokens.OpenSquareBracket:
					if (frame.lastExpressionStart.IsEmpty && token.kind == Tokens.OpenParenthesis)
						frame.lastExpressionStart = token.Location;
					frame = new Frame(frame);
					frame.parent.childType = frame.parent.type;
					frame.bracketType = '(';
					break;
				case Tokens.CloseParenthesis:
				case Tokens.CloseSquareBracket:
					if (frame.parent != null && frame.bracketType == '(') {
						frame.type = FrameType.Popped;
						frame = frame.parent;
					}
					break;
				case Tokens.LessThan:
					if (Tokens.ValidInsideTypeName[lastToken]) {
						frame = new Frame(frame);
						frame.bracketType = '<';
						frame.SetContext(ExpressionContext.Type);
					}
					break;
				case Tokens.GreaterThan:
					if (frame.parent != null && frame.bracketType == '<') {
						frame.type = FrameType.Popped;
						frame = frame.parent;
					}
					break;
				case Tokens.Dot:
				case Tokens.DoubleColon:
					// let the current expression continue
					break;
				default:
					if (Tokens.IdentifierTokens[token.kind]) {
						if (lastToken != Tokens.Dot && lastToken != Tokens.DoubleColon) {
							if (Tokens.ValidInsideTypeName[lastToken]) {
								frame.SetDefaultContext();
							}
							frame.lastExpressionStart = token.Location;
						}
					} else if (Tokens.SimpleTypeName[token.kind] || Tokens.ExpressionStart[token.kind]) {
						frame.lastExpressionStart = token.Location;
					} else {
						frame.lastExpressionStart = Location.Empty;
						frame.SetDefaultContext();
					}
					break;
			}
		}
		
		void TrackCurrentContext(Token token)
		{
			switch (token.kind) {
				case Tokens.Using:
					if (frame.type == FrameType.Global) {
						frame.SetContext(ExpressionContext.Namespace);
					}
					break;
				case Tokens.Throw:
					frame.SetExpectedType(projectContent.SystemTypes.Exception);
					break;
				case Tokens.New:
					frame.SetContext(ExpressionContext.TypeDerivingFrom(frame.expectedType, true));
					break;
				case Tokens.Namespace:
					frame.SetContext(ExpressionContext.IdentifierExpected);
					break;
				case Tokens.Assign:
					if (frame.type == FrameType.Global) {
						frame.SetContext(ExpressionContext.FullyQualifiedType);
						break;
					} else if (frame.type == FrameType.Enum) {
						frame.SetContext(ExpressionContext.Default);
						break;
					} else if (frame.type == FrameType.TypeDecl) {
						frame.SetContext(ExpressionContext.Default);
						frame.state = FrameState.Initializer;
						break;
					} else {
						goto default;
					}
				case Tokens.Colon:
					if (frame.state == FrameState.MethodDecl && lastToken == Tokens.CloseParenthesis) {
						frame.SetContext(ExpressionContext.BaseConstructorCall);
						//frame.childType = FrameType.Expression;
					} else {
						if (frame.childType == FrameType.TypeDecl || frame.childType == FrameType.Interface || frame.childType == FrameType.Enum) {
							frame.state = FrameState.InheritanceList;
							frame.SetDefaultContext();
						}
					}
					break;
				case Tokens.Class:
				case Tokens.Struct:
					if (frame.type == FrameType.Global || frame.type == FrameType.TypeDecl) {
						frame.childType = FrameType.TypeDecl;
						frame.SetContext(ExpressionContext.IdentifierExpected);
					}
					break;
				case Tokens.Interface:
					if (frame.type == FrameType.Global || frame.type == FrameType.TypeDecl) {
						frame.childType = FrameType.Interface;
						frame.SetContext(ExpressionContext.IdentifierExpected);
					}
					break;
				case Tokens.Enum:
					if (frame.type == FrameType.Global || frame.type == FrameType.TypeDecl) {
						frame.childType = FrameType.Enum;
						frame.SetContext(ExpressionContext.IdentifierExpected);
					}
					break;
				case Tokens.Delegate:
					if (frame.type == FrameType.Global || frame.type == FrameType.TypeDecl) {
						frame.childType = FrameType.ParameterList;
						frame.SetContext(ExpressionContext.Type);
					}
					break;
				case Tokens.Event:
					frame.SetContext(ExpressionContext.DelegateType);
					frame.childType = FrameType.Event;
					frame.state = FrameState.EventDecl;
					break;
				case Tokens.Comma:
					frame.lastExpressionStart = Location.Empty;
					frame.SetDefaultContext();
					break;
				case Tokens.CloseCurlyBrace:
				case Tokens.Semicolon:
					frame.state = FrameState.Normal;
					frame.SetDefaultContext();
					break;
				case Tokens.OpenParenthesis:
					if (frame.parent != null && frame.parent.state == FrameState.FieldDecl) {
						frame.type = FrameType.ParameterList;
						frame.SetContext(ExpressionContext.FirstParameterType);
						frame.parent.state = FrameState.MethodDecl;
						frame.parent.childType = FrameType.Statements;
					}
					break;
				default:
					if (Tokens.SimpleTypeName[token.kind]) {
						if (frame.type == FrameType.Interface || frame.type == FrameType.TypeDecl) {
							if (frame.state == FrameState.Normal) {
								frame.state = FrameState.FieldDecl;
								frame.childType = FrameType.Property;
							}
							frame.SetContext(ExpressionContext.IdentifierExpected);
						} else if (frame.type == FrameType.ParameterList) {
							frame.SetContext(ExpressionContext.IdentifierExpected);
						}
					}
					break;
			}
		}
		
		public ExpressionResult FindFullExpression(string text, int offset)
		{
			Init(text, offset);
			
			const int SEARCHING_OFFSET = 0;
			const int SEARCHING_END = 1;
			int state = SEARCHING_OFFSET;
			Frame resultFrame = frame;
			int resultStartOffset = -1;
			int resultEndOffset = -1;
			ExpressionContext context = ExpressionContext.Default;
			
			Token token;
			while ((token = lexer.NextToken()) != null) {
				if (token.kind == Tokens.EOF) break;
				
				if (state == SEARCHING_OFFSET) {
					if (targetPosition < token.Location) {
						resultFrame = frame;
						context = frame.context;
						resultStartOffset = LocationToOffset(frame.lastExpressionStart);
						if (resultStartOffset < 0)
							break;
						resultEndOffset = LocationToOffset(token.Location);
						state = SEARCHING_END;
					}
				}
				ApplyToken(token);
				if (state == SEARCHING_OFFSET) {
					if (targetPosition < token.EndLocation) {
						resultFrame = frame;
						context = frame.context;
						resultStartOffset = LocationToOffset(frame.lastExpressionStart);
						resultEndOffset = LocationToOffset(token.EndLocation);
						if (resultStartOffset < 0)
							break;
						state = SEARCHING_END;
					}
				} else if (state == SEARCHING_END) {
					if (resultFrame.type == FrameType.Popped ||
					    resultStartOffset != LocationToOffset(resultFrame.lastExpressionStart) ||
					    token.kind == Tokens.Dot || token.kind == Tokens.DoubleColon)
					{
						if (frame == resultFrame && Tokens.IdentifierTokens[token.kind]) {
							// the expression got aborted because of an identifier. This means the
							// expression was a type reference
							context = ExpressionContext.Type;
						}
						
						return new ExpressionResult(text.Substring(resultStartOffset, resultEndOffset - resultStartOffset), context);
					} else {
						resultEndOffset = LocationToOffset(token.EndLocation);
					}
				}
				lastToken = token.kind;
			}
			// offset is behind all tokens -> cannot find any expression
			return new ExpressionResult(null, frame.context);
		}
		
		public string RemoveLastPart(string expression)
		{
			Init(expression, expression.Length - 1);
			int lastValidPos = 0;
			Token token;
			while ((token = lexer.NextToken()) != null) {
				if (token.kind == Tokens.EOF) break;
				
				if (frame.parent == null) {
					if (token.kind == Tokens.Dot || token.kind == Tokens.DoubleColon
					    || token.kind == Tokens.OpenParenthesis || token.kind == Tokens.OpenSquareBracket)
					{
						lastValidPos = LocationToOffset(token.Location);
					}
				}
				ApplyToken(token);
				
				lastToken = token.kind;
			}
			return expression.Substring(0, lastValidPos);
		}
		
		#region Comment Filter and 'inside string watcher'
		
		// NOTE: FilterComments is not used anymore inside the ExpressionFinder, it should be moved
		// into another class / or removed completely if it is not required anymore.
		
		int initialOffset;
		public string FilterComments(string text, ref int offset)
		{
			if (text.Length <= offset)
				return null;
			this.initialOffset = offset;
			StringBuilder outText = new StringBuilder();
			int curOffset = 0;
			
			while (curOffset <= initialOffset) {
				char ch = text[curOffset];
				
				switch (ch) {
					case '@':
						if (curOffset + 1 < text.Length && text[curOffset + 1] == '"') {
							outText.Append(text[curOffset++]); // @
							outText.Append(text[curOffset++]); // "
							if (!ReadVerbatimString(outText, text, ref curOffset)) {
								return null;
							}
						}else{
							outText.Append(ch);
							++curOffset;
						}
						break;
					case '\'':
						outText.Append(ch);
						curOffset++;
						if(! ReadChar(outText, text, ref curOffset)) {
							return null;
						}
						break;
					case '"':
						outText.Append(ch);
						curOffset++;
						if (!ReadString(outText, text, ref curOffset)) {
							return null;
						}
						break;
					case '/':
						if (curOffset + 1 < text.Length && text[curOffset + 1] == '/') {
							offset    -= 2;
							curOffset += 2;
							if (!ReadToEOL(text, ref curOffset, ref offset)) {
								return null;
							}
						} else if (curOffset + 1 < text.Length && text[curOffset + 1] == '*') {
							offset    -= 2;
							curOffset += 2;
							if (!ReadMultiLineComment(text, ref curOffset, ref offset)) {
								return null;
							}
						} else {
							goto default;
						}
						break;
					case '#':
						if (!ReadToEOL(text, ref curOffset, ref offset)) {
							return null;
						}
						break;
					default:
						outText.Append(ch);
						++curOffset;
						break;
				}



			}
			
			return outText.ToString();

		}
		
		bool ReadToEOL(string text, ref int curOffset, ref int offset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				--offset;
				if (ch == '\n') {
					return true;
				}
			}
			return false;
		}
		
		bool ReadChar(StringBuilder outText, string text, ref int curOffset)
		{
			if (curOffset > initialOffset)
				return false;
			char first = text[curOffset++];
			outText.Append(first);
			if (curOffset > initialOffset)
				return false;
			char second = text[curOffset++];
			outText.Append(second);
			if (first == '\\') {
				// character is escape sequence, so read one char more
				char next;
				do {
					if (curOffset > initialOffset)
						return false;
					next = text[curOffset++];
					outText.Append(next);
					// unicode or hexadecimal character literals can have more content characters
				} while((second == 'u' || second == 'x') && char.IsLetterOrDigit(next));
			}
			return text[curOffset - 1] == '\'';
		}
		
		bool ReadString(StringBuilder outText, string text, ref int curOffset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				outText.Append(ch);
				if (ch == '"') {
					return true;
				} else if (ch == '\\') {
					if (curOffset <= initialOffset)
						outText.Append(text[curOffset++]);
				}
			}
			return false;
		}
		
		bool ReadVerbatimString(StringBuilder outText, string text, ref int curOffset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				outText.Append(ch);
				if (ch == '"') {
					if (curOffset < text.Length && text[curOffset] == '"') {
						outText.Append(text[curOffset++]);
					} else {
						return true;
					}
				}


			}
			return false;

		}
		
		bool ReadMultiLineComment(string text, ref int curOffset, ref int offset)
		{
			while (curOffset <= initialOffset) {
				char ch = text[curOffset++];
				--offset;
				if (ch == '*') {
					if (curOffset < text.Length && text[curOffset] == '/') {
						++curOffset;
						--offset;
						return true;
					}
				}
			}
			return false;
		}
		#endregion
	}
}
