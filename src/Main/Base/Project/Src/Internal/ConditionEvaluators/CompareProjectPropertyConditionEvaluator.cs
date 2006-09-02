// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
	/// 'InvariantCultureIgnoreCase'.
	/// </attribute>
	/// <example title="Check if the project output type in the active configuration is a Windows Application">
	/// &lt;Condition name = "CompareProjectProperty" property = "OutputType" equals = "WinExe"&gt;
	/// </example>
	public class CompareProjectPropertyConditionEvaluator: IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			AbstractProject project;
			if (caller is IProject) {
				project = caller as AbstractProject;
			} else {
				project = ProjectService.CurrentProject as AbstractProject;
			}
			if (project == null) {
				return false;
			}
			
			string comparisonTypeText = condition.Properties["comparisonType"];
			StringComparison comparisonType;
			if (string.IsNullOrEmpty(comparisonTypeText))
				comparisonType = StringComparison.InvariantCultureIgnoreCase;
			else
				comparisonType = (StringComparison)Enum.Parse(typeof(StringComparison), comparisonTypeText);
			
			return string.Equals(project.GetProperty(StringParser.Parse(condition.Properties["property"])),
			                     StringParser.Parse(condition.Properties["equals"]),
			                     comparisonType);
		}
	}
}
