// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Xml;


using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Tests if any project is active or if a project of a specific language is active.
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
			
			IProject project = ProjectService.CurrentProject;
			if (activeproject == "*") {
				return project != null;
			}
			
			return project != null && project.Language == activeproject;
		}
	}

}
