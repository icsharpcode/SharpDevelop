// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class ProjectBrowserRefresher
	{
		IPackageManagementProjectService projectService;
		IPackageManagementEvents packageManagementEvents;
		
		public ProjectBrowserRefresher(
			IPackageManagementProjectService projectService,
			IPackageManagementEvents packageManagementEvents)
		{
			this.projectService = projectService;
			this.packageManagementEvents = packageManagementEvents;
			
			packageManagementEvents.ParentPackageInstalled += ProjectChanged;
			packageManagementEvents.ParentPackageUninstalled += ProjectChanged;
		}
		
		void ProjectChanged(object sender, ParentPackageOperationEventArgs e)
		{
			projectService.RefreshProjectBrowser();
		}
	}
}
