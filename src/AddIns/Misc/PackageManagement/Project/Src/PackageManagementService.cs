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
		
		public event EventHandler ParentPackageInstalled;
		
		protected virtual void OnParentPackageInstalled()
		{
			if (ParentPackageInstalled != null) {
				ParentPackageInstalled(this, new EventArgs());
			}
		}
		
		public event EventHandler ParentPackageUninstalled;
		
		protected virtual void OnParentPackageUninstalled()
		{
			if (ParentPackageUninstalled != null) {
				ParentPackageUninstalled(this, new EventArgs());
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
		
		IPackageRepository CreateRecentPackageRepository()
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
			ISharpDevelopPackageManager packageManager = CreatePackageManagerForActiveProject(repository);
			return packageManager.ProjectManager;
		}
		
		public ISharpDevelopPackageManager CreatePackageManagerForActiveProject()
		{
			return CreatePackageManagerForActiveProject(ActivePackageRepository);
		}
		
		public ISharpDevelopPackageManager CreatePackageManagerForActiveProject(IPackageRepository packageRepository)
		{
			MSBuildBasedProject project = projectService.CurrentProject as MSBuildBasedProject;
			return CreatePackageManager(packageRepository, project);
		}
		
		ISharpDevelopPackageManager CreatePackageManager(IPackageRepository packageRepository, MSBuildBasedProject project)
		{
			return packageManagerFactory.CreatePackageManager(packageRepository, project);
		}
		
		public ISharpDevelopProjectManager CreateProjectManager(IPackageRepository packageRepository, MSBuildBasedProject project)
		{
			ISharpDevelopPackageManager packageManager = CreatePackageManager(packageRepository, project);
			return packageManager.ProjectManager;
		}
		
		public ISharpDevelopPackageManager CreatePackageManager(PackageSource packageSource, MSBuildBasedProject project)
		{
			IPackageRepository packageRepository = CreatePackageRepository(packageSource);
			return CreatePackageManager(packageRepository, project);
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
		
		public IPackageRepository CreatePackageRepository(PackageSource source)
		{
			return packageRepositoryCache.CreateRepository(source);
		}
		
		public InstallPackageAction CreateInstallPackageAction()
		{
			return new InstallPackageAction(this);
		}
		
		public UpdatePackageAction CreateUpdatePackageAction()
		{
			return new UpdatePackageAction(this);
		}
		
		public UninstallPackageAction CreateUninstallPackageAction()
		{
			return new UninstallPackageAction(this);
		}
		
		public void OnParentPackageInstalled(IPackage package)
		{
			projectService.RefreshProjectBrowser();
			RecentPackageRepository.AddPackage(package);
			OnParentPackageInstalled();
		}
		
		public void OnParentPackageUninstalled(IPackage package)
		{
			projectService.RefreshProjectBrowser();
			OnParentPackageUninstalled();
		}
	}
}
