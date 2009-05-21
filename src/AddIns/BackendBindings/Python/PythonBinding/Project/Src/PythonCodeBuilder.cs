// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;

namespace ICSharpCode.PythonBinding
{
	public class PythonCodeBuilder
	{
		StringBuilder codeBuilder = new StringBuilder();
		string indentString = "\t";
		int indent;
		
		public PythonCodeBuilder()
		{
		}
		
		public PythonCodeBuilder(int initialIndent)
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
			for (int i = 0; i < indent; ++i) {
				codeBuilder.Append(indentString);
			}			
			codeBuilder.Append(text);
		}
		
		public void AppendIndentedLine(string text)
		{
			AppendIndented(text + "\r\n");
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
	}
}
