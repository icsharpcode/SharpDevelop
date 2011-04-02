// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementService
	{
		event EventHandler PackageInstalled;
		event EventHandler PackageUninstalled;
		
		IPackageRepository CreateAggregatePackageRepository();
		IPackageRepository ActivePackageRepository { get; }
		IProjectManager ActiveProjectManager { get; }
		IPackageRepository RecentPackageRepository { get; }
		
		void InstallPackage(IPackageRepository repository, IPackage package, IEnumerable<PackageOperation> operations);
		void UninstallPackage(IPackageRepository repository, IPackage package);
		void UpdatePackage(IPackageRepository repository, IPackage package, IEnumerable<PackageOperation> operations);

		PackageManagementOptions Options { get; }
		
		bool HasMultiplePackageSources { get; }
		PackageSource ActivePackageSource { get; set; }
		
		IPackageManagementOutputMessagesView OutputMessagesView { get; }
	}
}
