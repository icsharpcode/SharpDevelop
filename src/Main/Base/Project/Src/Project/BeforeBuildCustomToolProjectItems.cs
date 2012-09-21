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
		IReadOnlyList<IProject> projects;
		
		public BeforeBuildCustomToolProjectItems(IReadOnlyList<IProject> projects)
		{
			this.projects = projects;
		}
		
		public IEnumerable<FileProjectItem> GetProjectItems()
		{
			return GetProjects()
				.SelectMany(p => GetConfiguredCustomToolProjectItems(p))
				.ToList();
		}
		
		IEnumerable<IProject> GetProjects()
		{
			return projects;
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
