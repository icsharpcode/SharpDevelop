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
