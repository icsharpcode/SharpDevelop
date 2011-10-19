// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
