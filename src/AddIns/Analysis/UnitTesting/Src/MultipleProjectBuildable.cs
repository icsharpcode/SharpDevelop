// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// IBuildable implementation that builds several projects.
	/// </summary>
	public class MultipleProjectBuildable : IBuildable
	{
		readonly IBuildable[] projects;
		
		public MultipleProjectBuildable(IEnumerable<IBuildable> projects)
		{
			this.projects = projects.ToArray();
		}
		
		public string Name {
			get { return string.Empty; }
		}
		
		public Solution ParentSolution {
			get { return projects.Length > 0 ? projects[0].ParentSolution : null; }
		}
		
		public ICollection<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			return projects;
		}
		
		public void StartBuild(ProjectBuildOptions buildOptions, IBuildFeedbackSink feedbackSink)
		{
			// SharpDevelop already has built our dependencies, so we're done immediately.
			feedbackSink.Done(true);
		}
		
		public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			return null;
		}
	}
}
