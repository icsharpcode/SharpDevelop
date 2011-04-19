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
	public class PackageManagementSolution : IPackageManagementSolution
	{
		IRegisteredPackageRepositories registeredPackageRepositories;
		IPackageManagerFactory packageManagerFactory;
		IPackageManagementProjectService projectService;
		IPackageManagementEvents packageManagementEvents;
		IPackageManagementProjectFactory projectFactory;
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents)
			: this(
				registeredPackageRepositories,
				new SharpDevelopPackageManagerFactory(),
				packageManagementEvents,
				new PackageManagementProjectService(),
				new PackageManagementProjectFactory())
		{
		}
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagerFactory packageManagerFactory,
			IPackageManagementEvents packageManagementEvents,
			IPackageManagementProjectService projectService,
			IPackageManagementProjectFactory projectFactory)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.packageManagementEvents = packageManagementEvents;
			this.packageManagerFactory = packageManagerFactory;
			this.projectFactory = projectFactory;
			this.projectService = projectService;
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
		
		public IPackageManagementProject GetActiveProject()
		{
			return GetActiveProject(ActivePackageRepository);
		}
		
		IPackageRepository ActivePackageRepository {
			get { return registeredPackageRepositories.ActiveRepository; }
		}
				
		public IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			MSBuildBasedProject activeProject = GetActiveMSBuildProject();
			return CreateProject(sourceRepository, activeProject);
		}
		
		public IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project)
		{
			return projectFactory.CreateProject(sourceRepository, project);
		}

		MSBuildBasedProject GetActiveMSBuildProject()
		{
			return projectService.CurrentProject as MSBuildBasedProject;
		}
		
		public IPackageManagementProject CreateProject(PackageSource source, MSBuildBasedProject project)
		{
			IPackageRepository sourceRepository = CreatePackageRepository(source);
			return CreateProject(sourceRepository, project);
		}
		
		IPackageRepository CreatePackageRepository(PackageSource source)
		{
			return registeredPackageRepositories.CreateRepository(source);
		}
	}
}
