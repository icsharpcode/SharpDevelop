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
			return project.HasProjectType(conditionGuid);
		}
	}
}
