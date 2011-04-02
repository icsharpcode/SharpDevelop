// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementService : IPackageManagementService
	{
		IPackageManagementOutputMessagesView outputMessagesView;
		PackageManagementOptions options;
		IPackageRepositoryCache packageRepositoryCache;
		IPackageManagerFactory packageManagerFactory;
		IPackageManagementProjectService projectService;
		IPackageRepository activePackageRepository;
		PackageSource activePackageSource;
		RecentPackageRepository recentPackageRepository;
		
		public PackageManagementService(
			PackageManagementOptions options,
			IPackageRepositoryCache packageRepositoryCache,
			IPackageManagerFactory packageManagerFactory,
			IPackageManagementProjectService projectService,
			IPackageManagementOutputMessagesView outputMessagesView)
		{
			this.options = options;
			this.packageRepositoryCache = packageRepositoryCache;
			this.packageManagerFactory = packageManagerFactory;
			this.projectService = projectService;
			this.outputMessagesView = outputMessagesView;
		}
		
		public PackageManagementService()
			: this(
				new PackageManagementOptions(),
				new PackageRepositoryCache(),
				new SharpDevelopPackageManagerFactory(),
				new PackageManagementProjectService(),
				new PackageManagementOutputMessagesView())
		{
		}
		
		public IPackageManagementOutputMessagesView OutputMessagesView {
			get { return outputMessagesView; }
		}
		
		public PackageManagementOptions Options {
			get { return options; }
		}
		
		public event EventHandler PackageInstalled;
		
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
		
		public IPackageRepository RecentPackageRepository {
			get {
				if (recentPackageRepository == null) {
					CreateRecentPackageRepository();
				}
				return recentPackageRepository;
			}
		}
		
		public IPackageRepository CreateRecentPackageRepository()
		{
			recentPackageRepository = new RecentPackageRepository(this);
			return recentPackageRepository;
		}
		
		public IPackageRepository ActivePackageRepository {
			get {
				if (activePackageRepository == null) {
					CreateActivePackageRepository();
				}
				return activePackageRepository;
			}
		}
		
		void CreateActivePackageRepository()
		{
			activePackageRepository = packageRepositoryCache.CreateRepository(ActivePackageSource);
		}
		
		public IProjectManager ActiveProjectManager {
			get { return GetActiveProjectManager(); }
		}
		
		IProjectManager GetActiveProjectManager()
		{
			IPackageRepository repository = ActivePackageRepository;
			ISharpDevelopPackageManager packageManager = CreatePackageManager(repository);
			return packageManager.ProjectManager;
		}
		
		public void InstallPackage(
			IPackageRepository packageRepository,
			IPackage package,
			IEnumerable<PackageOperation> operations)
		{
			ISharpDevelopPackageManager packageManager = CreatePackageManager(packageRepository);
			packageManager.InstallPackage(package, operations);
			projectService.RefreshProjectBrowser();
			RecentPackageRepository.AddPackage(package);
			OnPackageInstalled();
		}
		
		ISharpDevelopPackageManager CreatePackageManager(IPackageRepository packageRepository)
		{
			MSBuildBasedProject project = projectService.CurrentProject as MSBuildBasedProject;
			ISharpDevelopPackageManager packageManager = packageManagerFactory.CreatePackageManager(packageRepository, project);
			ConfigureLogger(packageManager);
			return packageManager;
		}
		
		void ConfigureLogger(ISharpDevelopPackageManager packageManager)
		{
			packageManager.Logger = outputMessagesView;
			packageManager.FileSystem.Logger = outputMessagesView;
			
			IProjectManager projectManager = packageManager.ProjectManager;
			projectManager.Logger = outputMessagesView;
			projectManager.Project.Logger = outputMessagesView;
		}
		
		public void UninstallPackage(IPackageRepository repository, IPackage package)
		{
			ISharpDevelopPackageManager packageManager = CreatePackageManager(repository);
			packageManager.UninstallPackage(package);
			projectService.RefreshProjectBrowser();
			OnPackageUninstalled();
		}
		
		public bool HasMultiplePackageSources {
			get { return options.PackageSources.HasMultiplePackageSources; }
		}
		
		public PackageSource ActivePackageSource {
			get {
				activePackageSource = options.ActivePackageSource;
				if (activePackageSource == null) {
					activePackageSource = options.PackageSources[0];
				}
				return activePackageSource;
			}
			set {
				if (activePackageSource != value) {
					activePackageSource = value;
					options.ActivePackageSource = value;
					activePackageRepository = null;
				}
			}
		}
		
		public IPackageRepository CreateAggregatePackageRepository()
		{
			IEnumerable<IPackageRepository> allRepositories = CreateAllRepositories();
			return new AggregateRepository(allRepositories);
		}
		
		IEnumerable<IPackageRepository> CreateAllRepositories()
		{
			foreach (PackageSource source in options.PackageSources) {
				yield return CreatePackageRepository(source);
			}
		}
		
		IPackageRepository CreatePackageRepository(PackageSource source)
		{
			return packageRepositoryCache.CreateRepository(source);
		}
		
		public void UpdatePackage(
			IPackageRepository repository,
			IPackage package,
			IEnumerable<PackageOperation> operations)
		{
			ISharpDevelopPackageManager packageManager = CreatePackageManager(repository);
			packageManager.UpdatePackage(package, operations);
			projectService.RefreshProjectBrowser();
			RecentPackageRepository.AddPackage(package);
			OnPackageInstalled();
		}
	}
}
