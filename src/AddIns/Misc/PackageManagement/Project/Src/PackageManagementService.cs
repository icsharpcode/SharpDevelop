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
		IRegisteredPackageRepositories registeredPackageRepositories;
		IPackageManagerFactory packageManagerFactory;
		IPackageManagementProjectService projectService;
		IPackageManagementEvents packageManagementEvents;
		
		public PackageManagementService(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents)
			: this(
				registeredPackageRepositories,
				new SharpDevelopPackageManagerFactory(),
				packageManagementEvents,
				new PackageManagementProjectService())
		{
		}
		
		public PackageManagementService(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagerFactory packageManagerFactory,
			IPackageManagementEvents packageManagementEvents,
			IPackageManagementProjectService projectService)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.packageManagementEvents = packageManagementEvents;
			this.packageManagerFactory = packageManagerFactory;
			this.projectService = projectService;
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
			return new InstallPackageAction(this, packageManagementEvents);
		}
		
		public UpdatePackageAction CreateUpdatePackageAction()
		{
			return new UpdatePackageAction(this, packageManagementEvents);
		}
		
		public UninstallPackageAction CreateUninstallPackageAction()
		{
			return new UninstallPackageAction(this, packageManagementEvents);
		}
	}
}
