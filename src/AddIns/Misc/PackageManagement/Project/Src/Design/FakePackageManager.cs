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
		
		public InstallPackageParameters ParametersPassedToInstallPackage;
		
		public IPackage PackagePassedToUninstallPackage;
		
		public struct InstallPackageParameters {
			public IPackage PackagePassedToInstallPackage;
			public bool IgnoreDependenciesPassedToInstallPackage;
			public IEnumerable<PackageOperation> PackageOperationsPassedToInstallPackage;
			
			public override string ToString()
			{
				return String.Format("Package: {0}, IgnoreDependencies: {1}",
					PackagePassedToInstallPackage,
					IgnoreDependenciesPassedToInstallPackage);
			}
		}
		
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
		
		public void InstallPackage(IPackage package, bool ignoreDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void InstallPackage(string packageId, Version version, bool ignoreDependencies)
		{
			throw new NotImplementedException();
		}
		
		public bool ForceRemovePassedToUninstallPackage;
		public bool RemoveDependenciesPassedToUninstallPackage;
		
		public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			PackagePassedToUninstallPackage = package;
			ForceRemovePassedToUninstallPackage = forceRemove;
			RemoveDependenciesPassedToUninstallPackage = removeDependencies;
			IsRefreshProjectBrowserCalledWhenUninstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
		
		public void UninstallPackage(string packageId, Version version, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(IPackage package)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(IPackage oldPackage, IPackage newPackage, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, Version version, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<PackageOperation> PackageOperationsPassedToInstallPackage;
		
		public void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations, bool ignoreDependencies)
		{
			PackagePassedToInstallPackage = package;
			
			ParametersPassedToInstallPackage = new InstallPackageParameters();
			ParametersPassedToInstallPackage.PackagePassedToInstallPackage = package;
			ParametersPassedToInstallPackage.PackageOperationsPassedToInstallPackage = operations;
			
			IgnoreDependenciesPassedToInstallPackage = ignoreDependencies;
			PackageOperationsPassedToInstallPackage = operations;
			
			IsRefreshProjectBrowserCalledWhenInstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
		
		public List<PackageOperation> PackageOperationsToReturnFromGetInstallPackageOperations = new List<PackageOperation>();
		public IPackage PackagePassedToGetInstallPackageOperations;
		public bool IgnoreDependenciesPassedToGetInstallPackageOperations;
		
		public IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies)
		{
			PackagePassedToGetInstallPackageOperations = package;
			IgnoreDependenciesPassedToGetInstallPackageOperations = ignoreDependencies;
			return PackageOperationsToReturnFromGetInstallPackageOperations;
		}
		
		public IPackage PackagePassedToUpdatePackage;
		public IEnumerable<PackageOperation> PackageOperationsPassedToUpdatePackage;
		public bool UpdateDependenciesPassedToUpdatePackage;
		
		public void UpdatePackage(IPackage package, IEnumerable<PackageOperation> operations, bool updateDependencies)
		{
			PackagePassedToUpdatePackage = package;
			PackageOperationsPassedToUpdatePackage = operations;
			UpdateDependenciesPassedToUpdatePackage = updateDependencies;
		}
		
		public void FirePackageInstalled(PackageOperationEventArgs e)
		{
			PackageInstalled(this, e);
		}
		
		public void FirePackageUninstalled(PackageOperationEventArgs e)
		{
			PackageUninstalled(this, e);
		}
	}
}
