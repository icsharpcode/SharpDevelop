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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingVariables : ITextTemplatingVariables
	{
		ITextTemplatingStringParser stringParser;
		
		public TextTemplatingVariables()
			: this(new TextTemplatingStringParser())
		{
		}
		
		public TextTemplatingVariables(
			ITextTemplatingStringParser stringParser)
		{
			this.stringParser = stringParser;
		}
		
		public string ExpandVariables(string name)
		{
			var variablesBuilder = new TextTemplatingVariablesStringBuilder(name, this);
			foreach (TextTemplatingVariableLocation variableLocation in GetVariables(name)) {
				variablesBuilder.AppendTextBeforeVariable(variableLocation);
				variablesBuilder.AppendVariable(variableLocation);
			}
			variablesBuilder.AppendRemaining();
			return variablesBuilder.ToString();
		}
		
		public IEnumerable<TextTemplatingVariableLocation> GetVariables(string text)
		{
			if (String.IsNullOrEmpty(text)) {
				yield break;
			}
			
			int currentIndex = 0;
			while (true) {
				TextTemplatingVariableLocation variableLocation = 
					FindVariable(text, currentIndex);
				if (variableLocation != null) {
					currentIndex = variableLocation.Index + variableLocation.Length;
					yield return variableLocation;
				} else {
					yield break;
				}
			}
		}
		
		TextTemplatingVariableLocation FindVariable(string text, int startIndex)
		{
			return TextTemplatingVariableLocation.FindVariable(text, startIndex);
		}
		
		public string GetValue(string name)
		{
			if (name == null) {
				return String.Empty;
			}
			
			string variableValue = stringParser.GetValue(name);
			if (IsDirectoryVariable(name)) {
				return AppendTrailingSlashIfMissing(variableValue);
			}
			return GetEmptyStringIfNull(variableValue);
		}
		
		bool IsDirectoryVariable(string name)
		{
			return TextTemplatingDirectoryVariable.IsDirectoryVariable(name);
		}
		
		string AppendTrailingSlashIfMissing(string variableValue)
		{
			return TextTemplatingDirectoryVariable.AppendTrailingSlashIfMissing(variableValue);
		}
		
		string GetEmptyStringIfNull(string variableValue)
		{
			if (variableValue != null) {
				return variableValue;
			}
			return String.Empty;
		}
	}
}
