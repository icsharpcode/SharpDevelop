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
		IRegisteredPackageRepositories registeredPackageRepositories;
		IPackageManagerFactory packageManagerFactory;
		IPackageManagementProjectService projectService;
		
		public PackageManagementService(
			PackageManagementOptions options,
			IPackageRepositoryCache packageRepositoryCache,
			IPackageManagerFactory packageManagerFactory,
			IPackageManagementProjectService projectService,
			IPackageManagementOutputMessagesView outputMessagesView)
			: this(
				new RegisteredPackageRepositories(packageRepositoryCache, options),
				packageManagerFactory,
				projectService,
				outputMessagesView)
		{
		}
		
		public PackageManagementService(IRegisteredPackageRepositories registeredPackageRepositories)
			: this(
				registeredPackageRepositories,
				new SharpDevelopPackageManagerFactory(),
				new PackageManagementProjectService(),
				new PackageManagementOutputMessagesView())
		{
		}
		
		public PackageManagementService(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagerFactory packageManagerFactory,
			IPackageManagementProjectService projectService,
			IPackageManagementOutputMessagesView outputMessagesView)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.packageManagerFactory = packageManagerFactory;
			this.projectService = projectService;
			this.outputMessagesView = outputMessagesView;
		}

		public IPackageManagementOutputMessagesView OutputMessagesView {
			get { return outputMessagesView; }
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
		
		IPackageRepository RecentPackageRepository {
			get { return registeredPackageRepositories.RecentPackageRepository; }
		}
		
		IPackageRepository ActivePackageRepository {
			get { return registeredPackageRepositories.ActiveRepository; }
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
		
		IPackageRepository CreatePackageRepository(PackageSource source)
		{
			return registeredPackageRepositories.CreateRepository(source);
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
