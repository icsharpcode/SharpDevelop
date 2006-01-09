// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Build.Tasks;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using System;

namespace ICSharpCode.MonoAddIn
{
	/// <summary>
	/// Determines whether the selected project is set to target the Mono framework.
	/// </summary>
	public class IsMonoProjectCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			IProject project = ProjectService.CurrentProject;
			if (project == null) {
				return false;
			}
			return MonoProjectContentLoader.IsMonoProject(project as MSBuildProject);
		}
	}
}
