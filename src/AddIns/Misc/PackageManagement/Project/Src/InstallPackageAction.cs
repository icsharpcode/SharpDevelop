// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class InstallPackageAction : ProcessPackageOperationsAction
	{
		public InstallPackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
		}
		
		public bool IgnoreDependencies { get; set; }
		
		protected override IEnumerable<PackageOperation> GetPackageOperations()
		{
			return Project.GetInstallPackageOperations(Package, this);
		}
		
		protected override void ExecuteCore()
		{
			Project.InstallPackage(Package, this);
			OnParentPackageInstalled();
		}
	}
}
