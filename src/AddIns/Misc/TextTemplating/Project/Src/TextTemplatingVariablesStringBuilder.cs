// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingVariablesStringBuilder
	{
		StringBuilder variablesBuilder = new StringBuilder();
		string unexpandedVariablesString;
		ITextTemplatingVariables templatingVariables;
		int currentIndex;
		
		public TextTemplatingVariablesStringBuilder(
			string unexpandedVariablesString,
			ITextTemplatingVariables templatingVariables)
		{
			this.unexpandedVariablesString = unexpandedVariablesString;
			this.templatingVariables = templatingVariables;
		}
		
		public void Append(string text)
		{
			variablesBuilder.Append(text);
		}
		
		public override string ToString()
		{
			return variablesBuilder.ToString();
		}
		
		public void AppendVariable(TextTemplatingVariableLocation variableLocation)
		{
			AppendVariableText(variableLocation);
			UpdateCurrentIndex(variableLocation);
		}
		
		public void AppendTextBeforeVariable(TextTemplatingVariableLocation variableLocation)
		{
			string textBeforeVariable = unexpandedVariablesString.Substring(currentIndex, variableLocation.Index);
			variablesBuilder.Append(textBeforeVariable);
		}
		
		void AppendVariableText(TextTemplatingVariableLocation variableLocation)
		{
			string variableValue = GetVariableValue(variableLocation);
			variablesBuilder.Append(variableValue);
		}
		
		void UpdateCurrentIndex(TextTemplatingVariableLocation variableLocation)
		{
			currentIndex = variableLocation.Index + variableLocation.Length;
		}
		
		string GetVariableValue(TextTemplatingVariableLocation variableLocation)
		{
			return templatingVariables.GetValue(variableLocation.VariableName);
		}
		
		public void AppendRemaining()
		{
			string textNotAppended = GetTextNotAppended();
			variablesBuilder.Append(textNotAppended);
		}
		
		string GetTextNotAppended()
		{
			return unexpandedVariablesString.Substring(currentIndex);
		}
	}
}
