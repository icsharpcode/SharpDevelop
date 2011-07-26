// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimeSelectProjectsViewModel
	{
		public DesignTimeSelectProjectsViewModel()
		{
			Projects = new ObservableCollection<IPackageManagementSelectedProject>();
			AddSelectedProject("AvalonEdit");
			AddSelectedProject("ICSharpCode.SharpDevelop");
			AddUnselectedProject("ICSharpCode.SharpDevelop.Dom");
			AddUnselectedProject("ICSharpCode.SharpDevelop.Widgets");
			AddSelectedProject("NRefactory");
		}
		
		void AddSelectedProject(string name)
		{
			AddProject(name, selected: true);
		}
		
		void AddUnselectedProject(string name)
		{
			AddProject(name, selected: false);
		}
		
		void AddProject(string name, bool selected)
		{
			var project = new FakeSelectedProject() {
				Name = name,
				IsSelected = selected
			};
			Projects.Add(project);
		}
		
		public ObservableCollection<IPackageManagementSelectedProject> Projects { get; private set; }
	}
}
