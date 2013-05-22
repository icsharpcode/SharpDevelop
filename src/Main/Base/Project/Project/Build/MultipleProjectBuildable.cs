// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// IBuildable implementation that builds several projects.
	/// </summary>
	public class MultipleProjectBuildable : IBuildable
	{
		readonly IBuildable[] projects;
		
		public MultipleProjectBuildable(IEnumerable<IBuildable> projects)
		{
			this.projects = projects.Where(p => p != null).ToArray();
		}
		
		public string Name {
			get { return string.Empty; }
		}
		
		public IEnumerable<IBuildable> GetBuildDependencies(ProjectBuildOptions buildOptions)
		{
			return projects;
		}
		
		public Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			// SharpDevelop already has built our dependencies, so we're done immediately.
			return Task.FromResult(true);
		}
		
		public ProjectBuildOptions CreateProjectBuildOptions(BuildOptions options, bool isRootBuildable)
		{
			return null;
		}
	}
}
