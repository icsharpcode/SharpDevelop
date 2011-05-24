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
		}
		
		public bool UpdateDependencies { get; set; }
		
		protected override IEnumerable<PackageOperation> GetPackageOperations()
		{
			return Project.GetInstallPackageOperations(Package, !UpdateDependencies);
		}
		
		protected override void ExecuteCore()
		{
			Project.UpdatePackage(Package, Operations, UpdateDependencies);
			OnParentPackageInstalled();
		}
	}
}
