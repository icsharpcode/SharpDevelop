// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement.EnvDTE;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageManagementProject
	{
		event EventHandler<PackageOperationEventArgs> PackageInstalled;
		event EventHandler<PackageOperationEventArgs> PackageUninstalled;
		event EventHandler<PackageOperationEventArgs> PackageReferenceAdded;
		event EventHandler<PackageOperationEventArgs> PackageReferenceRemoved;
		
		string Name { get; }
		ILogger Logger { get; set; }
		IPackageRepository SourceRepository { get; }
		
		Project ConvertToDTEProject();
		
		bool IsPackageInstalled(IPackage package);
		bool IsPackageInstalled(string packageId);
		
		IQueryable<IPackage> GetPackages();
		IEnumerable<IPackage> GetPackagesInReverseDependencyOrder();
		
		IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions);
		
		void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations, bool ignoreDependencies, bool allowPrereleaseVersions);
		void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations, bool updateDependencies, bool allowPrereleaseVersions);
		void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies);
		
		InstallPackageAction CreateInstallPackageAction();
		UninstallPackageAction CreateUninstallPackageAction();
		UpdatePackageAction CreateUpdatePackageAction();
	}
}
