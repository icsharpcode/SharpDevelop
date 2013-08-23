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
		event EventHandler<PackageOperationEventArgs> PackageReferenceRemoving;
		
		string Name { get; }
		ILogger Logger { get; set; }
		IPackageRepository SourceRepository { get; }
		
		Project ConvertToDTEProject();
		
		bool IsPackageInstalled(IPackage package);
		bool IsPackageInstalled(string packageId);
		bool HasOlderPackageInstalled(IPackage package);
		
		IQueryable<IPackage> GetPackages();
		IEnumerable<IPackage> GetPackagesInReverseDependencyOrder();
		
		IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, InstallPackageAction installAction);
		IEnumerable<PackageOperation> GetUpdatePackagesOperations(IEnumerable<IPackage> packages, IUpdatePackageSettings settings);
		
		void InstallPackage(IPackage package, InstallPackageAction installAction);
		void UpdatePackage(IPackage package, UpdatePackageAction updateAction);
		void UninstallPackage(IPackage package, UninstallPackageAction uninstallAction);
		void UpdatePackages(UpdatePackagesAction action);
		
		void UpdatePackageReference(IPackage package, IUpdatePackageSettings settings);
		
		InstallPackageAction CreateInstallPackageAction();
		UninstallPackageAction CreateUninstallPackageAction();
		UpdatePackageAction CreateUpdatePackageAction();
		UpdatePackagesAction CreateUpdatePackagesAction();
		
		void RunPackageOperations(IEnumerable<PackageOperation> expectedOperations);
	}
}
