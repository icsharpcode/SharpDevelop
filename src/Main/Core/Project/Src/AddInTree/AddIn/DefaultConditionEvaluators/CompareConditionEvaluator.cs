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

namespace ICSharpCode.Core
{
	/// <summary>
	/// Compares two strings.
	/// The strings are passed through the StringParser, so it is possible to compare
	/// SharpDevelop properties.<br/>
	/// Useful if you want to run a command only when a setting is active to prevent
	/// loading your addin if that setting isn't set.
	/// </summary>
	/// <attribute name="string">
	/// The first string.
	/// </attribute>
	/// <attribute name="equals">
	/// The second string.
	/// </attribute>
	/// <attribute name="comparisonType">
	/// The mode of the comparison: a field of the System.StringComparison enumeration. The default is
	/// 'OrdinalIgnoreCase'.
	/// </attribute>
	/// <example title="Check the value of a property in the PropertyService">
	/// &lt;Condition name = "Compare" string = "${property:SharpDevelop.FiletypesRegisterStartup}" equals = "True"&gt;
	/// </example>
	public class CompareConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object parameter, Condition condition)
		{
			string comparisonTypeText = condition.Properties["comparisonType"];
			StringComparison comparisonType;
			if (string.IsNullOrEmpty(comparisonTypeText))
				comparisonType = StringComparison.OrdinalIgnoreCase;
			else
				comparisonType = (StringComparison)Enum.Parse(typeof(StringComparison), comparisonTypeText);
			
			return string.Equals(StringParser.Parse(condition.Properties["string"]),
			                     StringParser.Parse(condition.Properties["equals"]),
			                     comparisonType);
		}
	}
}
