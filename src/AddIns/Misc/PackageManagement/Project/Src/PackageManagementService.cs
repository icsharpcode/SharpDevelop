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
		PackageManagementOptions options;
		ISharpDevelopPackageRepositoryFactory packageRepositoryFactory;
		IPackageManagerFactory packageManagerFactory;
		IPackageManagementProjectService projectService;
		IPackageRepository activePackageRepository;
		PackageSource activePackageSource;
		
		public PackageManagementService(PackageManagementOptions options,
			ISharpDevelopPackageRepositoryFactory packageRepositoryFactory,
			IPackageManagerFactory packageManagerFactory,
			IPackageManagementProjectService projectService)
		{
			this.options = options;
			this.packageRepositoryFactory = packageRepositoryFactory;
			this.packageManagerFactory = packageManagerFactory;
			this.projectService = projectService;
		}
		
		public PackageManagementService()
			: this(new PackageManagementOptions(),
				new SharpDevelopPackageRepositoryFactory(),
				new SharpDevelopPackageManagerFactory(),
				new PackageManagementProjectService())
		{
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
		
		public IPackageRepository ActivePackageRepository {
			get { return GetActivePackageRepository(); }
		}
		
		IPackageRepository GetActivePackageRepository()
		{
			if (activePackageRepository == null) {
				activePackageRepository = packageRepositoryFactory.CreateRepository(ActivePackageSource);
			}
			return activePackageRepository;
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
		
		public void InstallPackage(IPackageRepository packageRepository, IPackage package)
		{
			ISharpDevelopPackageManager packageManager = CreatePackageManager(packageRepository);
			packageManager.InstallPackage(package);
			projectService.RefreshProjectBrowser();
			OnPackageInstalled();
		}
		
		ISharpDevelopPackageManager CreatePackageManager(IPackageRepository packageRepository)
		{
			MSBuildBasedProject project = projectService.CurrentProject as MSBuildBasedProject;
			return packageManagerFactory.CreatePackageManager(packageRepository, project);
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
			return packageRepositoryFactory.CreateRepository(source);
		}
	}
}
