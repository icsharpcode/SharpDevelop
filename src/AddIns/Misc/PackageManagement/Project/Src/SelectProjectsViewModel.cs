// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.PackageManagement
{
	public class SelectProjectsViewModel
	{
		ObservableCollection<IPackageManagementSelectedProject> projects = 
			new ObservableCollection<IPackageManagementSelectedProject>();
		
		public SelectProjectsViewModel(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			AddProjects(projects);
		}
		
		void AddProjects(IEnumerable<IPackageManagementSelectedProject> projects)
		{
			foreach (IPackageManagementSelectedProject project in projects) {
				AddProject(project);
			}
		}
		
		void AddProject(IPackageManagementSelectedProject project)
		{
			projects.Add(project);
		}
		
		public ObservableCollection<IPackageManagementSelectedProject> Projects {
			get { return projects; }
		}
	}
}
