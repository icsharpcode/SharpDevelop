// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementService : IPackageManagementService
	{
		public event EventHandler PackageInstalled;
		
		PackageManagementOptions options = new PackageManagementOptions(new Properties());
		
		protected virtual void OnPackageInstalled()
		{
			if (PackageInstalled != null) {
				PackageInstalled(this, new EventArgs());
			}
		}
		
		public event EventHandler PackageUninstalled;
		
		protected virtual void OnPackageUninstalled()
		{
			if (PackageUninstalled != null) {
				PackageUninstalled(this, new EventArgs());
			}
		}
		
		public IPackageRepository RepositoryPassedToInstallPackage;
		public IPackage PackagePassedToInstallPackage;
		public bool IsInstallPackageCalled;
		
		public IPackageRepository RepositoryPassedToUninstallPackage;
		public IPackage PackagePassedToUninstallPackage;
		
		public FakeProjectManager FakeActiveProjectManager = new FakeProjectManager();
		public FakePackageRepository FakeActivePackageRepository = new FakePackageRepository();
		
		public FakePackageManagementService()
		{
			ActiveProjectManager = FakeActiveProjectManager;
			ActivePackageRepository = FakeActivePackageRepository;
			FakeActiveProjectManager.FakeSourceRepository = FakeActivePackageRepository;
		}
		
		public IPackageRepository ActivePackageRepository { get; set; }
		public IProjectManager ActiveProjectManager { get; set; }
		
		public void InstallPackage(IPackageRepository repository, IPackage package)
		{
			IsInstallPackageCalled = true;
			RepositoryPassedToInstallPackage = repository;
			PackagePassedToInstallPackage = package;
		}		
		
		public void UninstallPackage(IPackageRepository repository, IPackage package)
		{
			RepositoryPassedToUninstallPackage = repository;
			PackagePassedToUninstallPackage = package;
		}
		
		public void FirePackageInstalled()
		{
			OnPackageInstalled();
		}
		
		public void FirePackageUninstalled()
		{
			OnPackageUninstalled();
		}
		
		public void AddPackageToProjectLocalRepository(FakePackage package)
		{
			FakeActiveProjectManager.FakeLocalRepository.FakePackages.Add(package);
		}
		
		public PackageManagementOptions Options {
			get { return options; }
		}
	}
}
