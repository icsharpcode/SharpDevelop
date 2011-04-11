// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UninstallPackageAction : ProcessPackageAction
	{
		IPackageManagementService packageManagementService;
		
		public UninstallPackageAction(
			IPackageManagementService packageManagementService,
			IPackageManagementEvents packageManagementEvents)
			: base(packageManagementService, packageManagementEvents)
		{
			this.packageManagementService = packageManagementService;
		}
		
		public bool ForceRemove { get; set; }
		public bool RemoveDependencies { get; set; }

		protected override void BeforeExecute()
		{
			base.BeforeExecute();
		}
		
		protected override void ExecuteCore()
		{
			PackageManager.UninstallPackage(Package, ForceRemove, RemoveDependencies);
			OnParentPackageUninstalled();
		}
	}
}
