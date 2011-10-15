// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
