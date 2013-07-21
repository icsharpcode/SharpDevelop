// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Writes C# code into a TextWriter.
	/// </summary>
	public class TextWriterTokenWriter : ITokenWriter
	{
		readonly TextWriter textWriter;
		int indentation;
		bool needsIndent = true;
		bool isAtStartOfLine = true;

		public int Indentation {
			get {
				return this.indentation;
			}
			set {
				this.indentation = value;
			}
		}
		
		public string IndentationString { get; set; }
		
		public TextWriterTokenWriter(TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");
			this.textWriter = textWriter;
			this.IndentationString = "\t";
		}
		
		public void WriteIdentifier(Identifier identifier)
		{
			WriteIndentation();
			if (identifier.IsVerbatim)
				textWriter.Write('@');
			textWriter.Write(identifier.Name);
			isAtStartOfLine = false;
		}
		
		public void WriteKeyword(Role role, string keyword)
		{
			WriteIndentation();
			textWriter.Write(keyword);
			isAtStartOfLine = false;
		}
		
		public void WriteToken(Role role, string token)
		{
			WriteIndentation();
			textWriter.Write(token);
			isAtStartOfLine = false;
		}
		
		public void Space()
		{
			WriteIndentation();
			textWriter.Write(' ');
		}
		
		protected void WriteIndentation()
		{
			if (needsIndent) {
				needsIndent = false;
				for (int i = 0; i < indentation; i++) {
					textWriter.Write(this.IndentationString);
				}
			}
		}
		
		public void NewLine()
		{
			textWriter.WriteLine();
			needsIndent = true;
			isAtStartOfLine = true;
		}
		
		public void Indent()
		{
			indentation++;
		}
		
		public void Unindent()
		{
			indentation--;
		}
		
		public void WriteComment(CommentType commentType, string content)
		{
			WriteIndentation();
			switch (commentType) {
				case CommentType.SingleLine:
					textWriter.Write("//");
					textWriter.WriteLine(content);
					needsIndent = true;
					isAtStartOfLine = true;
					break;
				case CommentType.MultiLine:
					textWriter.Write("/*");
					textWriter.Write(content);
					textWriter.Write("*/");
					isAtStartOfLine = false;
					break;
				case CommentType.Documentation:
					textWriter.Write("///");
					textWriter.WriteLine(content);
					needsIndent = true;
					isAtStartOfLine = true;
					break;
				default:
					textWriter.Write(content);
					break;
			}
		}
		
		public void WritePreProcessorDirective(PreProcessorDirectiveType type, string argument)
		{
			// pre-processor directive must start on its own line
			if (!isAtStartOfLine)
				NewLine();
			WriteIndentation();
			textWriter.Write('#');
			textWriter.Write(type.ToString().ToLowerInvariant());
			if (!string.IsNullOrEmpty(argument)) {
				textWriter.Write(' ');
				textWriter.Write(argument);
			}
			NewLine();
		}
		
		public static string PrintPrimitiveValue(object value)
		{
			TextWriter writer = new StringWriter();
			TextWriterTokenWriter tokenWriter = new TextWriterTokenWriter(writer);
			tokenWriter.WritePrimitiveValue(value);
			return writer.ToString();
		}
		
		public void WritePrimitiveValue(object value)
		{
			if (value == null) {
				// usually NullReferenceExpression should be used for this, but we'll handle it anyways
				textWriter.Write("null");
				return;
			}
			
			if (value is bool) {
				if ((bool)value) {
					textWriter.Write("true");
				} else {
					textWriter.Write("false");
				}
				return;
			}
			
			if (value is string) {
				textWriter.Write("\"" + ConvertString(value.ToString()) + "\"");
			} else if (value is char) {
				textWriter.Write("'" + ConvertCharLiteral((char)value) + "'");
			} else if (value is decimal) {
				textWriter.Write(((decimal)value).ToString(NumberFormatInfo.InvariantInfo) + "m");
			} else if (value is float) {
				float f = (float)value;
				if (float.IsInfinity(f) || float.IsNaN(f)) {
					// Strictly speaking, these aren't PrimitiveExpressions;
					// but we still support writing these to make life easier for code generators.
					textWriter.Write("float");
					WriteToken(Roles.Dot, ".");
					if (float.IsPositiveInfinity(f)) {
						textWriter.Write("PositiveInfinity");
					} else if (float.IsNegativeInfinity(f)) {
						textWriter.Write("NegativeInfinity");
					} else {
						textWriter.Write("NaN");
					}
					return;
				}
				if (f == 0 && 1 / f == float.NegativeInfinity) {
					// negative zero is a special case
					// (again, not a primitive expression, but it's better to handle
					// the special case here than to do it in all code generators)
					textWriter.Write("-");
				}
				textWriter.Write(f.ToString("R", NumberFormatInfo.InvariantInfo) + "f");
			} else if (value is double) {
				double f = (double)value;
				if (double.IsInfinity(f) || double.IsNaN(f)) {
					// Strictly speaking, these aren't PrimitiveExpressions;
					// but we still support writing these to make life easier for code generators.
					textWriter.Write("double");
					textWriter.Write(".");
					if (double.IsPositiveInfinity(f)) {
						textWriter.Write("PositiveInfinity");
					} else if (double.IsNegativeInfinity(f)) {
						textWriter.Write("NegativeInfinity");
					} else {
						textWriter.Write("NaN");
					}
					return;
				}
				if (f == 0 && 1 / f == double.NegativeInfinity) {
					// negative zero is a special case
					// (again, not a primitive expression, but it's better to handle
					// the special case here than to do it in all code generators)
					textWriter.Write("-");
				}
				string number = f.ToString("R", NumberFormatInfo.InvariantInfo);
				if (number.IndexOf('.') < 0 && number.IndexOf('E') < 0) {
					number += ".0";
				}
				textWriter.Write(number);
			} else if (value is IFormattable) {
				StringBuilder b = new StringBuilder ();
//				if (primitiveExpression.LiteralFormat == LiteralFormat.HexadecimalNumber) {
//					b.Append("0x");
//					b.Append(((IFormattable)val).ToString("x", NumberFormatInfo.InvariantInfo));
//				} else {
					b.Append(((IFormattable)value).ToString(null, NumberFormatInfo.InvariantInfo));
//				}
				if (value is uint || value is ulong) {
					b.Append("u");
				}
				if (value is long || value is ulong) {
					b.Append("L");
				}
				textWriter.Write(b.ToString());
			} else {
				textWriter.Write(value.ToString());
			}
		}
		
		static string ConvertCharLiteral(char ch)
		{
			if (ch == '\'') {
				return "\\'";
			}
			return ConvertChar(ch);
		}
		
		/// <summary>
		/// Gets the escape sequence for the specified character.
		/// </summary>
		/// <remarks>This method does not convert ' or ".</remarks>
		public static string ConvertChar(char ch)
		{
			switch (ch) {
				case '\\':
					return "\\\\";
				case '\0':
					return "\\0";
				case '\a':
					return "\\a";
				case '\b':
					return "\\b";
				case '\f':
					return "\\f";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\t':
					return "\\t";
				case '\v':
					return "\\v";
				default:
					if (char.IsControl(ch) || char.IsSurrogate(ch) ||
					    // print all uncommon white spaces as numbers
					    (char.IsWhiteSpace(ch) && ch != ' ')) {
						return "\\u" + ((int)ch).ToString("x4");
					} else {
						return ch.ToString();
					}
			}
		}
		
		/// <summary>
		/// Converts special characters to escape sequences within the given string.
		/// </summary>
		public static string ConvertString(string str)
		{
			StringBuilder sb = new StringBuilder ();
			foreach (char ch in str) {
				if (ch == '"') {
					sb.Append("\\\"");
				} else {
					sb.Append(ConvertChar(ch));
				}
			}
			return sb.ToString();
		}
		
		public virtual void StartNode(AstNode node)
		{
			// Write out the indentation, so that overrides of this method
			// can rely use the current output length to identify the position of the node
			// in the output.
			WriteIndentation();
		}
		
		public virtual void EndNode(AstNode node)
		{
		}
	}
}
