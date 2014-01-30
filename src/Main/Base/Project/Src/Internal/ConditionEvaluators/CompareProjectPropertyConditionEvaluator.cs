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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Compares a project property with a string.<br/>
	/// Uses the conditions' caller as project; or the current project, if the caller is not a project.
	/// The property name and string are passed through the StringParser.
	/// </summary>
	/// <attribute name="property">
	/// The name of the MSBuild property to read the value from.
	/// </attribute>
	/// <attribute name="equals">
	/// The second string.
	/// </attribute>
	/// <attribute name="comparisonType">
	/// The mode of the comparison: a field of the System.StringComparison enumeration. The default is
	/// 'OrdinalIgnoreCase'.
	/// </attribute>
	/// <example title="Check if the project output type in the active configuration is a Windows Application">
	/// &lt;Condition name = "CompareProjectProperty" property = "OutputType" equals = "WinExe"&gt;
	/// </example>
	public class CompareProjectPropertyConditionEvaluator: IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			MSBuildBasedProject project;
			if (caller is IProject) {
				project = caller as MSBuildBasedProject;
			} else {
				project = ProjectService.CurrentProject as MSBuildBasedProject;
			}
			if (project == null) {
				return false;
			}
			
			string comparisonTypeText = condition.Properties["comparisonType"];
			StringComparison comparisonType;
			if (string.IsNullOrEmpty(comparisonTypeText))
				comparisonType = StringComparison.OrdinalIgnoreCase;
			else
				comparisonType = (StringComparison)Enum.Parse(typeof(StringComparison), comparisonTypeText);
			
			return string.Equals(project.GetEvaluatedProperty(StringParser.Parse(condition.Properties["property"])),
			                     StringParser.Parse(condition.Properties["equals"]),
			                     comparisonType);
		}
	}
}
