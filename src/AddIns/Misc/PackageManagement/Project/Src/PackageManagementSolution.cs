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
		IPackageManagementProjectService projectService;
		IPackageManagementEvents packageManagementEvents;
		IPackageManagementProjectFactory projectFactory;
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents)
			: this(
				registeredPackageRepositories,
				packageManagementEvents,
				new PackageManagementProjectService(),
				new PackageManagementProjectFactory(packageManagementEvents))
		{
		}
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents,
			IPackageManagementProjectService projectService,
			IPackageManagementProjectFactory projectFactory)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.packageManagementEvents = packageManagementEvents;
			this.projectFactory = projectFactory;
			this.projectService = projectService;
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
		
		MSBuildBasedProject GetActiveMSBuildProject()
		{
			return projectService.CurrentProject as MSBuildBasedProject;
		}
		
		IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project)
		{
			return projectFactory.CreateProject(sourceRepository, project);
		}
		
		IPackageRepository CreatePackageRepository(PackageSource source)
		{
			return registeredPackageRepositories.CreateRepository(source);
		}
		
		public IPackageManagementProject GetProject(PackageSource source, string projectName)
		{
			MSBuildBasedProject msbuildProject = GetMSBuildProject(projectName);
			return CreateProject(source, msbuildProject);
		}
		
		MSBuildBasedProject GetMSBuildProject(string name)
		{
			var openProjects = new OpenMSBuildProjects(projectService);
			return openProjects.FindProject(name);
		}
		
		IPackageManagementProject CreateProject(PackageSource source, MSBuildBasedProject project)
		{
			IPackageRepository sourceRepository = CreatePackageRepository(source);
			return CreateProject(sourceRepository, project);
		}
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			MSBuildBasedProject msbuildProject = GetMSBuildProject(projectName);
			return CreateProject(sourceRepository, msbuildProject);
		}
		
		public IEnumerable<IProject> GetMSBuildProjects()
		{
			return projectService.GetOpenProjects();
		}
		
		public bool IsOpen {
			get { return projectService.OpenSolution != null; }
		}
	}
}
