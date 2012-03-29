// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectBehaviorSupportedConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object owner, Condition condition)
		{
			Guid conditionGuid;
			if (!Guid.TryParse(condition.Properties["guid"], out conditionGuid))
				return true;
			
			string guidString;
			if (owner is IProject)
				guidString = FindGuidInProject((IProject)owner);
			else if (ProjectService.CurrentProject != null)
				guidString = FindGuidInProject(ProjectService.CurrentProject);
			else
				return false;
			
			Guid result;
			foreach (string guid in guidString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)) {
				if (Guid.TryParse(guid, out result) && conditionGuid == result)
					return true;
			}
			
			return false;
		}

		string FindGuidInProject(IProject project)
		{
			if (project is MSBuildBasedProject) {
				string guid = ((MSBuildBasedProject)project).GetEvaluatedProperty("ProjectTypeGuids");
				if (!string.IsNullOrEmpty(guid))
					return guid;
			} else if (project is UnknownProject || project is MissingProject) {
				// don't return any GUID for projects that could not be loaded
				return string.Empty;
			}
			
			return project.TypeGuid;
		}
	}
}
