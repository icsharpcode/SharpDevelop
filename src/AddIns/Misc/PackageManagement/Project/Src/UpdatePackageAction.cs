// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackageAction : ProcessPackageAction
	{
		public UpdatePackageAction(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents)
			: base(solution, packageManagementEvents)

		{
			UpdateDependencies = true;
		}
		
		public UpdatePackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
			UpdateDependencies = true;
		}
		
		public IEnumerable<PackageOperation> Operations { get; set; }
		public bool UpdateDependencies { get; set; }
		
		protected override void BeforeExecute()
		{
			base.BeforeExecute();
			GetPackageOperationsIfMissing();
		}
				
		void GetPackageOperationsIfMissing()
		{
			if (Operations == null) {
				Operations = Project.GetInstallPackageOperations(Package, !UpdateDependencies);
			}
		}
		
		protected override void ExecuteCore()
		{
			Project.UpdatePackage(Package, Operations, UpdateDependencies);
			OnParentPackageInstalled();
		}
	}
}
