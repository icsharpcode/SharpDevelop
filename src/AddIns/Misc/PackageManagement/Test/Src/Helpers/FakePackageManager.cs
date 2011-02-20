// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
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
		
		public IFileSystem FileSystem { get; set; }
		public IPackageRepository LocalRepository { get; set; }
		public ILogger Logger { get; set; }
		public IPackageRepository SourceRepository { get; set; }
		public ISharpDevelopProjectManager ProjectManager { get; set; }
		
		public FakePackageManager()
		{
			ProjectManager = FakeProjectManager;
		}
		
		public void InstallPackage(IPackage package)
		{
			InstallPackage(package, false);
		}
		
		public void InstallPackage(IPackage package, bool ignoreDependencies)
		{
			PackagePassedToInstallPackage = package;
			IgnoreDependenciesPassedToInstallPackage = ignoreDependencies;
			
			ParametersPassedToInstallPackage = new InstallPackageParameters();
			ParametersPassedToInstallPackage.PackagePassedToInstallPackage = package;
			ParametersPassedToInstallPackage.IgnoreDependenciesPassedToInstallPackage = ignoreDependencies;
			
			IsRefreshProjectBrowserCalledWhenInstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
		
		public void InstallPackage(string packageId, Version version, bool ignoreDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(IPackage package, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(string packageId, Version version, bool forceRemove, bool removeDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UninstallPackage(IPackage package)
		{
			PackagePassedToUninstallPackage = package;
			IsRefreshProjectBrowserCalledWhenUninstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
		
		public void UpdatePackage(IPackage oldPackage, IPackage newPackage, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void UpdatePackage(string packageId, Version version, bool updateDependencies)
		{
			throw new NotImplementedException();
		}
		
		public void InstallPackage(IPackage package, IEnumerable<PackageOperation> operations)
		{
			PackagePassedToInstallPackage = package;
			
			ParametersPassedToInstallPackage = new InstallPackageParameters();
			ParametersPassedToInstallPackage.PackagePassedToInstallPackage = package;
			ParametersPassedToInstallPackage.PackageOperationsPassedToInstallPackage = operations;
			
			IsRefreshProjectBrowserCalledWhenInstallPackageCalled = FakeProjectService.IsRefreshProjectBrowserCalled;
		}
	}
}
