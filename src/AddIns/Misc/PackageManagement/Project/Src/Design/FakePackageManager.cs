// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManager : ISharpDevelopPackageManager
	{
		public FakeProjectManager FakeProjectManager = new FakeProjectManager();
		public FakePackageManagementProjectService FakeProjectService = new FakePackageManagementProjectService();
		
		public bool IsRefreshProjectBrowserCalledWhenInstallPackageCalled;
		public bool IsRefreshProjectBrowserCalledWhenUninstallPackageCalled;
		
		public IPackage PackagePassedToInstallPackage;
		public bool IgnoreDependenciesPassedToInstallPackage;
		public bool AllowPrereleaseVersionsPassedToInstallPackage;
		
		public IPackage PackagePassedToUninstallPackage;
		
		#pragma warning disable 67
		public event EventHandler<PackageOperationEventArgs> PackageInstalled;
		public event EventHandler<PackageOperationEventArgs> PackageInstalling;
		public event EventHandler<PackageOperationEventArgs> PackageUninstalled;
		public event EventHandler<PackageOperationEventArgs> PackageUninstalling;
		#pragma warning restore 67
		
		public IFileSystem FileSystem {
			get { return FakeFileSystem; }
			set { FakeFileSystem = value as FakeFileSystem; }
		}
		
		public FakeFileSystem FakeFileSystem = new FakeFileSystem();
		
		public IPackageRepository LocalRepository { get; set; }
		public ILogger Logger { get; set; }
		public IPackageRepository SourceRepository { get; set; }
		public ISharpDevelopProjectManager ProjectManager { get; set; }
		
		public FakePackageRepository FakeSourceRepository = new FakePackageRepository();
		
		public FakePackageManager()
		{
			ProjectManager = FakeProjectManager;
			SourceRepository = FakeSourceRepository;
		}
		
		public bool ForceRemovePassedToUninstallPackage;
		public bool RemoveDependenciesPassedToUninstallPackage;
		
		public void UninstallPackage(IPackage package, UninstallPackageAction uninstallAction)
		{
			PackagePassedToUninstallPackage = package;
			ForceRemovePassedToUninstallPackage = uninstallAction.ForceRemove;
			RemoveDependenciesPassedToUninstallPackage = uninstallAction.RemoveDependencies;
			IsRefreshProjectBrowserCalledWhenUninstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
		
		public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<PackageOperation> PackageOperationsPassedToInstallPackage;
		
		public void InstallPackage(IPackage package, InstallPackageAction installAction)
		{
			PackagePassedToInstallPackage = package;
			
			IgnoreDependenciesPassedToInstallPackage = installAction.IgnoreDependencies;
			PackageOperationsPassedToInstallPackage = installAction.Operations;
			AllowPrereleaseVersionsPassedToInstallPackage = installAction.AllowPrereleaseVersions;
			
			IsRefreshProjectBrowserCalledWhenInstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
		
		public List<PackageOperation> PackageOperationsToReturnFromGetInstallPackageOperations = new List<PackageOperation>();
		public IPackage PackagePassedToGetInstallPackageOperations;
		public bool IgnoreDependenciesPassedToGetInstallPackageOperations;
		public bool AllowPrereleaseVersionsPassedToGetInstallPackageOperations;
		
		public IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, InstallPackageAction installAction)
		{
			PackagePassedToGetInstallPackageOperations = package;
			IgnoreDependenciesPassedToGetInstallPackageOperations = installAction.IgnoreDependencies;
			AllowPrereleaseVersionsPassedToGetInstallPackageOperations = installAction.AllowPrereleaseVersions;
			return PackageOperationsToReturnFromGetInstallPackageOperations;
		}
		
		public IPackage PackagePassedToUpdatePackage;
		public IEnumerable<PackageOperation> PackageOperationsPassedToUpdatePackage;
		public bool UpdateDependenciesPassedToUpdatePackage;
		
		public void UpdatePackage(IPackage package, UpdatePackageAction updateAction)
		{
			PackagePassedToUpdatePackage = package;
			PackageOperationsPassedToUpdatePackage = updateAction.Operations;
			UpdateDependenciesPassedToUpdatePackage = updateAction.UpdateDependencies;
			AllowPrereleaseVersionsPassedToInstallPackage = updateAction.AllowPrereleaseVersions;
		}
		
		public void FirePackageInstalled(PackageOperationEventArgs e)
		{
			PackageInstalled(this, e);
		}
		
		public void FirePackageUninstalled(PackageOperationEventArgs e)
		{
			PackageUninstalled(this, e);
		}
		
		public IPackagePathResolver PathResolver {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void UpdatePackage(IPackage newPackage, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, IVersionSpec versionSpec, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void InstallPackage(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void InstallPackage(string packageId, SemanticVersion version, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(IPackage newPackage, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, SemanticVersion version, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, IVersionSpec versionSpec, bool updateDependencies, bool allowPrereleaseVersions)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(string packageId, SemanticVersion version, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
	}
}
