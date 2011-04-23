// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class InstallPackageAction : ProcessPackageAction
	{
		public InstallPackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
		}
		
		public IEnumerable<PackageOperation> Operations { get; set; }
		public bool IgnoreDependencies { get; set; }
		
		protected override void BeforeExecute()
		{
			base.BeforeExecute();
			GetPackageOperationsIfMissing();
		}
				
		void GetPackageOperationsIfMissing()
		{
			if (Operations == null) {
				Operations = Project.GetInstallPackageOperations(Package, IgnoreDependencies);
			}
		}
		
		protected override void ExecuteCore()
		{
			Project.InstallPackage(Package, Operations, IgnoreDependencies);
			OnParentPackageInstalled();
		}
	}
}
