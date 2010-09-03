// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Tests if any project is active or if a project of a specific language is active.
	/// Can also be used to test the type of the project passed as caller to the condition
	/// - when a project is passed as caller, the language of that project is tested; otherwise
	/// the language of the active project is tested.
	/// </summary>
	/// <attribute name="activeproject">
	/// The language name the project should have.
	/// "*" to test if any project is active.
	/// </attribute>
	/// <example title="Test if any project is active">
	/// &lt;Condition name = "ProjectActive" activeproject="*"&gt;
	/// </example>
	/// <example title="Test if a C# project is active">
	/// &lt;Condition name = "ProjectActive" activeproject="C#"&gt;
	/// </example>
	public class ProjectActiveConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			string activeproject = condition.Properties["activeproject"];
			
			IProject project = (caller as IProject) ?? ProjectService.CurrentProject;
			if (activeproject == "*") {
				return project != null;
			}
			
			return project != null && project.Language == activeproject;
		}
	}
}
