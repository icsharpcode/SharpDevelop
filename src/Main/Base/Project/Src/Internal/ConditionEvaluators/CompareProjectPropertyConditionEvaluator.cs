// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
