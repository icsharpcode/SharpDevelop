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
			
		ILogger Logger { get; set; }
		IPackageRepository SourceRepository { get; }
		
		Project ConvertToDTEProject();
		
		bool IsInstalled(IPackage package);
		
		IQueryable<IPackage> GetPackages();
		
		IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies);
		
		void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations, bool ignoreDependencies);
		void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations, bool updateDependencies);
		void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies);
		
		InstallPackageAction CreateInstallPackageAction();
		UninstallPackageAction CreateUninstallPackageAction();
		UpdatePackageAction CreateUpdatePackageAction();
	}
}
