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
	sealed class SharpDevelopSolutionSnapshot : DefaultSolutionSnapshot, ISolutionSnapshotWithProjectMapping
	{
		Dictionary<IProject, IProjectContent> projectContentSnapshots = new Dictionary<IProject, IProjectContent>();
		Lazy<ICompilation> dummyCompilation;
		
		public SharpDevelopSolutionSnapshot(IEnumerable<IProject> projects)
		{
			dummyCompilation = new Lazy<ICompilation>(() => new SimpleCompilation(this, MinimalCorlib.Instance));
			if (projects != null) {
				foreach (IProject project in projects) {
					var pc = project.ProjectContent;
					if (pc != null)
						projectContentSnapshots.Add(project, pc);
				}
			}
		}
		
		public IProjectContent GetProjectContent(IProject project)
		{
			if (project == null)
				return null;
			IProjectContent pc;
			if (projectContentSnapshots.TryGetValue(project, out pc))
				return pc;
			else
				return null;
		}
		
		public IProject GetProject(IAssembly assembly)
		{
			if (assembly != null)
				return GetProject(assembly.UnresolvedAssembly as IProjectContent);
			else
				return null;
		}
		
		public IProject GetProject(IProjectContent projectContent)
		{
			if (projectContent == null)
				return null;
			foreach (var pair in projectContentSnapshots) {
				if (pair.Value == projectContent)
					return pair.Key;
			}
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
