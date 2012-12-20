// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UninstallPackageAction : ProcessPackageAction
	{
		public UninstallPackageAction(
			IPackageManagementProject project,
			IPackageManagementEvents packageManagementEvents)
			: base(project, packageManagementEvents)
		{
			this.AllowPrereleaseVersions = true;
		}
		
		public bool ForceRemove { get; set; }
		public bool RemoveDependencies { get; set; }

		protected override void BeforeExecute()
		{
			base.BeforeExecute();
		}
		
		protected override void ExecuteCore()
		{
			Project.UninstallPackage(Package, this);
			OnParentPackageUninstalled();
		}
		
		public override bool HasPackageScriptsToRun()
		{
			var files = new PackageFiles(Package);
			return files.HasUninstallPackageScript();
		}
	}
}
