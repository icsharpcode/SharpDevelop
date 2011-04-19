// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UninstallPackageAction : ProcessPackageAction
	{
		IPackageManagementSolution solution;
		
		public UninstallPackageAction(
			IPackageManagementSolution solution,
			IPackageManagementEvents packageManagementEvents)
			: base(solution, packageManagementEvents)
		{
			this.solution = solution;
		}
		
		public bool ForceRemove { get; set; }
		public bool RemoveDependencies { get; set; }

		protected override void BeforeExecute()
		{
			base.BeforeExecute();
		}
		
		protected override void ExecuteCore()
		{
			Project.UninstallPackage(Package, ForceRemove, RemoveDependencies);
			OnParentPackageUninstalled();
		}
	}
}
