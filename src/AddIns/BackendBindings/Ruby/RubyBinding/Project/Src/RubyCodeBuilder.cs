// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Text;

namespace ICSharpCode.RubyBinding
{
	public class RubyCodeBuilder
	{
		StringBuilder codeBuilder = new StringBuilder();
		string indentString = "\t";
		int indent;
		bool insertedCreateComponentsContainer;
		
		public RubyCodeBuilder()
		{
		}
		
		public RubyCodeBuilder(int initialIndent)
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
		/// Inserts the following line of code before all the other lines of code:
		/// 
		/// "self._components = System.ComponentModel.Container()"
		/// 
		/// This line will only be inserted once. Multiple calls to this method will only result in one
		/// line of code being inserted.
		/// </summary>
		public void InsertCreateComponentsContainer()
		{
			if (!insertedCreateComponentsContainer) {
				InsertIndentedLine("self._components = " + typeof(Container).FullName + "()");
				insertedCreateComponentsContainer = true;
			}
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
		
		public void AppendLineIfPreviousLineIsEndStatement()
		{
			string previousLine = GetPreviousLine().Trim();
			if (previousLine.Equals("end", StringComparison.InvariantCultureIgnoreCase)) {
				AppendLine();
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
