// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;

namespace ICSharpCode.Scripting
{
	public class ScriptingCodeBuilder
	{
		StringBuilder codeBuilder = new StringBuilder();
		string indentString = "\t";
		int indent;
		
		public ScriptingCodeBuilder()
		{
		}
		
		public ScriptingCodeBuilder(int initialIndent)
		{
			indent = initialIndent;
		}
		
				/// <summary>
		/// Gets or sets the string used for indenting.
		/// </summary>
		public string IndentString {
			get { return indentString; }
			set { indentString = value; }
		}
		
		/// <summary>
		/// Returns the code.
		/// </summary>
		public override string ToString()
		{
			return codeBuilder.ToString();
		}
				
		/// <summary>
		/// Returns true if the previous line contains code. If the previous line contains a
		/// comment or is an empty line then it returns false;
		/// </summary>
		public bool PreviousLineIsCode {
			get { 				
				string code = ToString();
				int end = MoveToPreviousLineEnd(code, code.Length - 1);
				if (end > 0) {
					int start = MoveToPreviousLineEnd(code, end);
					string line = code.Substring(start + 1, end - start).Trim();
					return (line.Length > 0) && (!line.Trim().StartsWith("#"));
				}
				return false;
			}
		}
		
		public string GetPreviousLine()
		{
			string code = ToString();
			int end = MoveToPreviousLineEnd(code, code.Length - 1);
			if (end > 0) {
				int start = MoveToPreviousLineEnd(code, end);
				return code.Substring(start + 1, end - start);
			}
			return String.Empty;
		}
		
				/// <summary>
		/// Appends text at the end of the current code.
		/// </summary>
		public void Append(string text)
		{
			codeBuilder.Append(text);
		}
		
		/// <summary>
		/// Appends carriage return and line feed to the existing text.
		/// </summary>
		public void AppendLine()
		{
			Append("\r\n");
		}
		
		/// <summary>
		/// Appends the text indented.
		/// </summary>
		public void AppendIndented(string text)
		{
			codeBuilder.Append(GetIndentString());
			codeBuilder.Append(text);
		}

		/// <summary>
		/// Inserts a new line at the start of the code before everything else.
		/// </summary>
		public void InsertIndentedLine(string text)
		{
			text = GetIndentString() + text + "\r\n";
			codeBuilder.Insert(0, text, 1);
		}
		
				/// <summary>
		/// Inserts the text with a carriage return and newline at the end.
		/// </summary>
		public void AppendIndentedLine(string text)
		{
			AppendIndented(text + "\r\n");
		}

		public void AppendLineIfPreviousLineIsCode()
		{
			if (PreviousLineIsCode) {
				codeBuilder.AppendLine();
			}
		}		

		/// <summary>
		/// Appends the specified text to the end of the previous line.
		/// </summary>
		public void AppendToPreviousLine(string text)
		{
			string code = ToString();
			int end = MoveToPreviousLineEnd(code, code.Length - 1);
			if (end > 0) {
				codeBuilder.Insert(end + 1, text);
			}
		}
		
		public void TrimEnd()
		{
			string trimmedText = codeBuilder.ToString().TrimEnd();
			codeBuilder = new StringBuilder();
			codeBuilder.Append(trimmedText);
		}
		
		public void IncreaseIndent()
		{
			indent++;
		}
		
		public void DecreaseIndent()
		{
			indent--;
		}
		
		public int Indent {
			get { return indent; }
		}

		/// <summary>
		/// Gets the length of the current code string.
		/// </summary>
		public int Length {
			get { return codeBuilder.Length; }
		}
		
		string GetIndentString()
		{
			StringBuilder currentIndentString = new StringBuilder();
			for (int i = 0; i < indent; ++i) {
				currentIndentString.Append(indentString);
			}
			return currentIndentString.ToString();
		}
		
		/// <summary>
		/// Returns the index of the end of the previous line.
		/// </summary>
		/// <param name="index">This is the index to start working backwards from.</param>
		int MoveToPreviousLineEnd(string code, int index)
		{
			while (index >= 0) {
				if (code[index] == '\r') {
					return index - 1;
				}
				--index;
			}
			return -1;
		}
	}
}
