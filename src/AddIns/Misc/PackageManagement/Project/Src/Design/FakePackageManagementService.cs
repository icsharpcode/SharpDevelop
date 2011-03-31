// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementService : IPackageManagementService
	{
		public event EventHandler PackageInstalled;
		
		PackageManagementOptions options = new PackageManagementOptions(new Properties());
		
		public List<PackageOperation> PackageOperationsPassedToInstallPackage = new List<PackageOperation>();
		
		public FakePackageManagementProjectService FakeProjectService = new FakePackageManagementProjectService();
		
		public IPackageManagementProjectService ProjectService {
			get { return FakeProjectService; }
		}
		
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
		
		public FakeProjectManager FakeActiveProjectManager { get; set; }
		
		public FakePackageRepository FakeActivePackageRepository {
			get { return ActivePackageRepository as FakePackageRepository; }
			set { ActivePackageRepository = value; }
		}
		
		public FakePackageManagementService()
		{
			FakeActiveProjectManager = new FakeProjectManager();
			FakeActivePackageRepository = new FakePackageRepository();
			FakeActiveProjectManager.FakeSourceRepository = FakeActivePackageRepository;
		}
		
		public virtual IPackageRepository ActivePackageRepository { get; set; }
		
		public virtual IProjectManager ActiveProjectManager {
			get { return FakeActiveProjectManager; }
		}
		
		public virtual void InstallPackage(IPackageRepository repository, IPackage package, IEnumerable<PackageOperation> operations)
		{
			IsInstallPackageCalled = true;
			RepositoryPassedToInstallPackage = repository;
			PackagePassedToInstallPackage = package;
			PackageOperationsPassedToInstallPackage.AddRange(operations);
		}
		
		public virtual void UninstallPackage(IPackageRepository repository, IPackage package)
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
		
		public void ClearPackageSources()
		{
			options.PackageSources.Clear();
		}
		
		public PackageSource AddOnePackageSource()
		{
			return AddOnePackageSource("Test");
		}
		
		public PackageSource AddOnePackageSource(string name)
		{
			var source = new PackageSource("http://sharpdevelop.codeplex.com", name);
			options.PackageSources.Add(source);
			return source;
		}
		
		public bool HasMultiplePackageSources { get; set; }
		
		public void AddPackageSources(IEnumerable<PackageSource> sources)
		{
			options.PackageSources.AddRange(sources);
		}
		
		public PackageSource ActivePackageSource { get; set; }
		
		public FakePackageRepository FakeAggregateRepository = new FakePackageRepository();
		
		public IPackageRepository CreateAggregatePackageRepository()
		{
			return FakeAggregateRepository;
		}
		
		public FakePackageManagementOutputMessagesView FakeOutputMessagesView = new FakePackageManagementOutputMessagesView();
		
		public IPackageManagementOutputMessagesView OutputMessagesView {
			get { return FakeOutputMessagesView; }
		}
		
		public FakePackageRepository FakeRecentPackageRepository = new FakePackageRepository();
		
		public IPackageRepository RecentPackageRepository {
			get { return FakeRecentPackageRepository; }
		}
		
		public FakePackageRepository FakePackageRepositoryToReturnFromCreatePackageRepository = new FakePackageRepository();
		public PackageSource PackageSourcePassedToCreatePackageRepository;
		
		public IPackageRepository CreatePackageRepository(PackageSource source)
		{
			PackageSourcePassedToCreatePackageRepository = source;
			return FakePackageRepositoryToReturnFromCreatePackageRepository;
		}
		
		public FakeProjectManager FakeProjectManagerToReturnFromCreateProjectManager = new FakeProjectManager();
		public IPackageRepository PackageRepositoryPassedToCreateProjectManager;
		public MSBuildBasedProject ProjectPassedToCreateProjectManager;
		
		public ISharpDevelopProjectManager CreateProjectManager(IPackageRepository repository, MSBuildBasedProject project)
		{
			PackageRepositoryPassedToCreateProjectManager = repository;
			ProjectPassedToCreateProjectManager = project;
			
			return FakeProjectManagerToReturnFromCreateProjectManager;
		}
		
		public FakePackageManager FakePackageManagerToReturnFromCreatePackageManagerForActiveProject =
			new FakePackageManager();
		
		public virtual ISharpDevelopPackageManager CreatePackageManagerForActiveProject()
		{
			return FakePackageManagerToReturnFromCreatePackageManagerForActiveProject;
		}
		
		public FakePackage AddFakePackageWithVersionToAggregrateRepository(string version)
		{
			return AddFakePackageWithVersionToAggregrateRepository("Test", version);
		}
		
		public FakePackage AddFakePackageWithVersionToAggregrateRepository(string id, string version)
		{
			var package = FakePackage.CreatePackageWithVersion(id, version);
			FakeAggregateRepository.FakePackages.Add(package);
			return package;
		}
		
		public string PackageIdPassedToInstallPackage;
		public PackageSource PackageSourcePassedToInstallPackage;
		public MSBuildBasedProject ProjectPassedToInstallPackage;
		public bool IgnoreDependenciesPassedToInstallPackage;
		public Version VersionPassedToInstallPackage;
		
		public void InstallPackage(
			string packageId,
			Version version,
			MSBuildBasedProject project,
			PackageSource packageSource,
			bool ignoreDependencies)
		{
			PackageIdPassedToInstallPackage = packageId;
			VersionPassedToInstallPackage = version;
			ProjectPassedToInstallPackage = project;
			PackageSourcePassedToInstallPackage = packageSource;
			IgnoreDependenciesPassedToInstallPackage = ignoreDependencies;
		}
		
		public MSBuildBasedProject FakeProjectToReturnFromGetProject;
		public string NamePassedToGetProject;
		
		public MSBuildBasedProject GetProject(string name)
		{
			NamePassedToGetProject = name;
			return FakeProjectToReturnFromGetProject;
		}
		
		public string PackageIdPassedToUninstallPackage;
		public MSBuildBasedProject ProjectPassedToUninstallPackage;
		public Version VersionPassedToUninstallPackage;
		public PackageSource PackageSourcePassedToUninstallPackage;
		public bool ForceRemovePassedToUninstallPackage;
		public bool RemoveDependenciesPassedToUninstallPackage;
		
		public void UninstallPackage(
			string packageId,
			Version version,
			MSBuildBasedProject project,
			PackageSource packageSource,
			bool forceRemove,
			bool removeDependencies)
		{
			PackageIdPassedToUninstallPackage = packageId;
			VersionPassedToUninstallPackage = version;
			ProjectPassedToUninstallPackage = project;
			PackageSourcePassedToUninstallPackage = packageSource;
			ForceRemovePassedToUninstallPackage = forceRemove;
			RemoveDependenciesPassedToUninstallPackage = removeDependencies;
		}
	}
}
