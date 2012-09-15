// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project
{
	public class BeforeBuildCustomToolProjectItems
	{
		IBuildable buildable;
		
		public BeforeBuildCustomToolProjectItems(IBuildable buildable)
		{
			this.buildable = buildable;
		}
		
		public IEnumerable<FileProjectItem> GetProjectItems()
		{
			return GetProjects()
				.SelectMany(p => GetConfiguredCustomToolProjectItems(p))
				.ToList();
		}
		
		IEnumerable<IProject> GetProjects()
		{
			IProject project = buildable as IProject;
			if (project != null) {
				return new IProject[] { project };
			}
			
			var solution = buildable as Solution;
			if (solution != null) {
				return solution.Projects;
			}
			
			return new IProject[0];
		}
		
		IEnumerable<FileProjectItem> GetConfiguredCustomToolProjectItems(IProject project)
		{
			var fileNameFilter = new BeforeBuildCustomToolFileNameFilter(project);
			if (!fileNameFilter.Any()) {
				return new FileProjectItem[0];
			}
			
			return project
				.Items
				.OfType<FileProjectItem>()
				.Where(item => fileNameFilter.IsMatch(item.Include));
		}
	}
}
