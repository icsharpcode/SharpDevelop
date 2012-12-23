// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackageAction : ProcessPackageOperationsAction
	{
		public UpdatePackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
			UpdateDependencies = true;
			UpdateIfPackageDoesNotExistInProject = true;
		}
		
		public bool UpdateDependencies { get; set; }
		public bool UpdateIfPackageDoesNotExistInProject { get; set; }
		
		protected override IEnumerable<PackageOperation> GetPackageOperations()
		{
			var installAction = Project.CreateInstallPackageAction();
			installAction.AllowPrereleaseVersions = AllowPrereleaseVersions;
			installAction.IgnoreDependencies = !UpdateDependencies;
			return Project.GetInstallPackageOperations(Package, installAction);
		}
		
		protected override void ExecuteCore()
		{
			if (ShouldUpdatePackage()) {
				Project.UpdatePackage(Package, this);
				OnParentPackageInstalled();
			}
		}
		
		bool ShouldUpdatePackage()
		{
			if (!UpdateIfPackageDoesNotExistInProject) {
				return PackageIdExistsInProject();
			}
			return true;
		}
	}
}
