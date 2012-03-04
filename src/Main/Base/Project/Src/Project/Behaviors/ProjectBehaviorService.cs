// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectBehaviorService
	{
		const string AddInPath = "/SharpDevelop/Workbench/ProjectBehaviors";
		
		public static ProjectBehavior LoadBehaviorsForProject(IProject project, ProjectBehavior defaultBehavior)
		{
			List<ProjectBehavior> behaviors = AddInTree.BuildItems<ProjectBehavior>(AddInPath, project, false);
			ProjectBehavior first = null, current = null;
			foreach (var behavior in behaviors) {
				behavior.SetProject(project);
				if (first == null)
					first = behavior;
				else
					current.SetNext(behavior);
				current = behavior;
			}
			if (current == null)
				return defaultBehavior;
			current.SetNext(defaultBehavior);
			return first;
		}
	}
}
