// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class InstalledPackageViewModel : PackageViewModel
	{
		public InstalledPackageViewModel(
			IPackageFromRepository package,
			PackageManagementSelectedProjects selectedProjects,
			IPackageManagementEvents packageManagementEvents,
			IPackageActionRunner actionRunner,
			ILogger logger)
			: base(package, selectedProjects, packageManagementEvents, actionRunner, logger)
		{
		}
		
		public override IList<ProcessPackageAction> GetProcessPackageActionsForSelectedProjects(
			IList<IPackageManagementSelectedProject> selectedProjects)
		{
			var actions = new List<ProcessPackageAction>();
			foreach (IPackageManagementSelectedProject selectedProject in selectedProjects) {
				ProcessPackageAction action = CreatePackageAction(selectedProject);
				actions.Add(action);
			}
			return actions;
		}
		
		ProcessPackageAction CreatePackageAction(IPackageManagementSelectedProject selectedProject)
		{
			if (selectedProject.IsSelected) {
				return base.CreateInstallPackageAction(selectedProject);
			}
			return base.CreateUninstallPackageAction(selectedProject);
		}
		
		protected override bool AnyProjectsSelected(IList<IPackageManagementSelectedProject> projects)
		{
			return true;
		}
	}
}
