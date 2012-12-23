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
		IPackageManagementProjectFactory projectFactory;
		ISolutionPackageRepositoryFactory solutionPackageRepositoryFactory;
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementEvents packageManagementEvents)
			: this(
				registeredPackageRepositories,
				new PackageManagementProjectService(),
				new PackageManagementProjectFactory(packageManagementEvents),
				new SolutionPackageRepositoryFactory())
		{
		}
		
		public PackageManagementSolution(
			IRegisteredPackageRepositories registeredPackageRepositories,
			IPackageManagementProjectService projectService,
			IPackageManagementProjectFactory projectFactory,
			ISolutionPackageRepositoryFactory solutionPackageRepositoryFactory)
		{
			this.registeredPackageRepositories = registeredPackageRepositories;
			this.projectFactory = projectFactory;
			this.projectService = projectService;
			this.solutionPackageRepositoryFactory = solutionPackageRepositoryFactory;
		}
		
		public string FileName {
			get { return OpenSolution.FileName; }
		}
		
		Solution OpenSolution {
			get { return projectService.OpenSolution; }
		}
		
		public IPackageManagementProject GetActiveProject()
		{
			if (HasActiveProject()) {
				return GetActiveProject(ActivePackageRepository);
			}
			return null;
		}
		
		bool HasActiveProject()
		{
			return GetActiveMSBuildBasedProject() != null;
		}
		
		public IProject GetActiveMSBuildProject()
		{
			return projectService.CurrentProject;
		}
		
		IPackageRepository ActivePackageRepository {
			get { return registeredPackageRepositories.ActiveRepository; }
		}
				
		public IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			MSBuildBasedProject activeProject = GetActiveMSBuildBasedProject();
			if (activeProject != null) {
				return CreateProject(sourceRepository, activeProject);
			}
			return null;
		}
		
		MSBuildBasedProject GetActiveMSBuildBasedProject()
		{
			return GetActiveMSBuildProject() as MSBuildBasedProject;
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
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, IProject project)
		{
			var msbuildProject = project as MSBuildBasedProject;
			return CreateProject(sourceRepository, msbuildProject);
		}
		
		public IEnumerable<IProject> GetMSBuildProjects()
		{
			return projectService.GetOpenProjects();
		}
		
		public bool IsOpen {
			get { return OpenSolution != null; }
		}
		
		public bool HasMultipleProjects()
		{
			return projectService.GetOpenProjects().Count() > 1;
		}
		
		public bool IsPackageInstalled(IPackage package)
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			return repository.IsInstalled(package);
		}
		
		ISolutionPackageRepository CreateSolutionPackageRepository()
		{
			return solutionPackageRepositoryFactory.CreateSolutionPackageRepository(OpenSolution);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			return repository.GetPackages();
		}
		
		public string GetInstallPath(IPackage package)
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			return repository.GetInstallPath(package);
		}
		
		public IEnumerable<IPackage> GetPackagesInReverseDependencyOrder()
		{
			ISolutionPackageRepository repository = CreateSolutionPackageRepository();
			return repository.GetPackagesByReverseDependencyOrder();
		}
		
		public IEnumerable<IPackageManagementProject> GetProjects(IPackageRepository sourceRepository)
		{
			foreach (MSBuildBasedProject msbuildProject in GetMSBuildProjects()) {
				yield return projectFactory.CreateProject(sourceRepository, msbuildProject);
			}
		}
	}
}
