// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementSelectedProject : IPackageManagementSelectedProject
	{
		public PackageManagementSelectedProject(IPackageManagementProject project)
			: this(project, selected: false)
		{
		}
		
		public PackageManagementSelectedProject(
			IPackageManagementProject project,
			bool selected)
			: this(project, selected, enabled: true)
		{
		}
		
		public PackageManagementSelectedProject(
			IPackageManagementProject project,
			bool selected,
			bool enabled)
		{
			this.Project = project;
			this.Name = Project.Name;
			this.IsSelected = selected;
			this.IsEnabled = enabled;
		}
		
		public IPackageManagementProject Project { get; private set; }
		public string Name { get; private set; }
		
		public bool IsSelected { get; set; }
		public bool IsEnabled { get; set; }
	}
}
