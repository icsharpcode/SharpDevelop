// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementService
	{
		event EventHandler ParentPackageInstalled;
		event EventHandler ParentPackageUninstalled;
		
		IPackageRepository CreateAggregatePackageRepository();
		IPackageRepository CreatePackageRepository(PackageSource source);
		ISharpDevelopProjectManager CreateProjectManager(IPackageRepository repository, MSBuildBasedProject project);
		ISharpDevelopPackageManager CreatePackageManagerForActiveProject();
		ISharpDevelopPackageManager CreatePackageManagerForActiveProject(IPackageRepository packageRepository);
		ISharpDevelopPackageManager CreatePackageManager(PackageSource packageSource, MSBuildBasedProject project);
		
		IPackageRepository ActivePackageRepository { get; }
		IProjectManager ActiveProjectManager { get; }
		IPackageRepository RecentPackageRepository { get; }
		
		InstallPackageAction CreateInstallPackageAction();
		UninstallPackageAction CreateUninstallPackageAction();
		UpdatePackageAction CreateUpdatePackageAction();
		
		void OnParentPackageInstalled(IPackage package);
		void OnParentPackageUninstalled(IPackage package);
		
		PackageManagementOptions Options { get; }
		
		bool HasMultiplePackageSources { get; }
		PackageSource ActivePackageSource { get; set; }
		
		IPackageManagementOutputMessagesView OutputMessagesView { get; }
	}
}
