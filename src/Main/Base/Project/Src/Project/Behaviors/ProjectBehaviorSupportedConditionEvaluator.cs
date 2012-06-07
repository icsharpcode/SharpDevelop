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
			
			IProject project = owner as IProject ?? ProjectService.CurrentProject;
			if (project == null)
				return false;
			// TODO: simplify this once HasProjectType() is part of IProject
			AbstractProject p2 = project as AbstractProject;
			if (p2 != null) {
				return p2.HasProjectType(conditionGuid);
			} else {
				Guid projectGuid;
				if (Guid.TryParse(project.TypeGuid, out projectGuid))
					return conditionGuid == projectGuid;
				else
					return false;
			}
		}
	}
}
