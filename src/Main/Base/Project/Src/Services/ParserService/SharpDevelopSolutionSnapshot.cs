// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	public class SharpDevelopSolutionSnapshot : DefaultSolutionSnapshot
	{
		Dictionary<IProject, IProjectContent> projectContentSnapshots = new Dictionary<IProject, IProjectContent>();
		Lazy<ICompilation> dummyCompilation;
		
		public SharpDevelopSolutionSnapshot()
			: this(ProjectService.OpenSolution)
		{
			dummyCompilation = new Lazy<ICompilation>(() => new SimpleCompilation(this, MinimalCorlib.Instance));
		}
		
		public SharpDevelopSolutionSnapshot(Solution solution)
		{
			if (solution != null) {
				foreach (IProject project in solution.Projects) {
					var pc = project.ProjectContent;
					if (pc != null)
						projectContentSnapshots.Add(project, pc);
				}
			}
		}
		
		public IProjectContent GetProjectContent(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			IProjectContent pc;
			if (projectContentSnapshots.TryGetValue(project, out pc))
				return pc;
			else
				return null;
		}
		
		public ICompilation GetCompilation(IProject project)
		{
			IProjectContent pc;
			if (project != null && projectContentSnapshots.TryGetValue(project, out pc))
				return GetCompilation(pc);
			else
				return dummyCompilation.Value;
		}
	}
}
