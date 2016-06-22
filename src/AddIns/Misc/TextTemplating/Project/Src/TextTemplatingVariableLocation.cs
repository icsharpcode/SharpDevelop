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

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingVariableLocation
	{
		public TextTemplatingVariableLocation()
			: this(String.Empty, 0, 0)
		{
		}
		
		public TextTemplatingVariableLocation(
			string variableName,
			int index,
			int length)
		{
			this.VariableName = variableName;
			this.Index = index;
			this.Length = length;
		}
		
		public static TextTemplatingVariableLocation CreateFromString(
			string text,
			int unexpandedVariableStartIndex,
			int unexpandedVariableEndIndex)
		{
			int variableNameStart = unexpandedVariableStartIndex + 2;
			int unexpandedLength = unexpandedVariableEndIndex - unexpandedVariableStartIndex + 1;
			string variableName = text.Substring(variableNameStart, unexpandedVariableEndIndex - variableNameStart);
			return new TextTemplatingVariableLocation(variableName, unexpandedVariableStartIndex, unexpandedLength);
		}
		
		public string VariableName { get; set; }
		public int Index { get; set; }
		public int Length { get; set; }
		
		public override bool Equals(object obj)
		{
			var rhs = obj as TextTemplatingVariableLocation;
			if (rhs != null) {
				return (VariableName == rhs.VariableName) &&
					(Index == rhs.Index) &&
					(Length == rhs.Length);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public override string ToString()
		{
			return String.Format(
				"VariableName: {0}, Index: {1}, Length: {2}",
				VariableName, Index, Length);
		}
		
		public static TextTemplatingVariableLocation FindVariable(string text, int startSearchIndex)
		{
			int start = text.IndexOf("$(", startSearchIndex);
			if (start >= 0) {
				int end = text.IndexOf(")", start);
				if (end >= 0) {
					return CreateFromString(text, start, end);
				}
			}
			return null;
		}
	}
}
