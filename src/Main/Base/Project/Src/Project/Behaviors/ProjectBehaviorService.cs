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
