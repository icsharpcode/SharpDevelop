// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageManagementSolution : IPackageManagementSolution
	{
		public void AddPackageToActiveProjectLocalRepository(FakePackage package)
		{
			FakeActiveProject.FakePackages.Add(package);
		}
		
		public FakePackage AddPackageToActiveProjectLocalRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			AddPackageToActiveProjectLocalRepository(package);
			return package;
		}
		
		public int GetActiveProjectCallCount;
		public FakePackageManagementProject FakeActiveProject = new FakePackageManagementProject();
		public bool IsGetActiveProjectWithNoParametersCalled;
		
		public virtual IPackageManagementProject GetActiveProject()
		{
			GetActiveProjectCallCount++;
			IsGetActiveProjectWithNoParametersCalled = true;
			
			return FakeActiveProject;
		}
		
		public IPackageRepository RepositoryPassedToGetActiveProject;
		
		public virtual IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			RepositoryPassedToGetActiveProject = sourceRepository;
			return FakeActiveProject;
		}
		
		public FakePackageManagementProject FakeProjectToReturnFromGetProject =
			new FakePackageManagementProject();
		
		public PackageSource PackageSourcePassedToGetProject;
		public string ProjectNamePassedToGetProject;
		
		public IPackageManagementProject GetProject(PackageSource source, string projectName)
		{
			PackageSourcePassedToGetProject = source;
			ProjectNamePassedToGetProject = projectName;
			return FakeProjectToReturnFromGetProject;
		}
		
		public IPackageRepository RepositoryPassedToGetProject;
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			RepositoryPassedToGetProject = sourceRepository;
			ProjectNamePassedToGetProject = projectName;
			return FakeProjectToReturnFromGetProject;
		}
		
		public IProject ProjectPassedToGetProject;
		public List<IProject> ProjectsPassedToGetProject = new List<IProject>();
		public Dictionary<string, FakePackageManagementProject> FakeProjectsToReturnFromGetProject
			= new Dictionary<string, FakePackageManagementProject>();
		
		public virtual IPackageManagementProject GetProject(IPackageRepository sourceRepository, IProject project)
		{
			RepositoryPassedToGetProject = sourceRepository;
			ProjectPassedToGetProject = project;
			ProjectsPassedToGetProject.Add(project);
			FakePackageManagementProject fakeProject = null;
			if (FakeProjectsToReturnFromGetProject.TryGetValue(project.Name, out fakeProject)) {
				return fakeProject;
			}
			return FakeProjectToReturnFromGetProject;
		}
		
		public IProject FakeActiveMSBuildProject;
		
		public IProject GetActiveMSBuildProject()
		{
			return FakeActiveMSBuildProject;
		}
		
		public List<IProject> FakeMSBuildProjects = new List<IProject>();
		
		public IEnumerable<IProject> GetMSBuildProjects()
		{
			return FakeMSBuildProjects;
		}
		
		public bool IsOpen { get; set; }
		
		public bool HasMultipleProjects()
		{
			return FakeMSBuildProjects.Count > 1;
		}
		
		public string FileName { get; set; }
		
		public List<FakePackage> FakeInstalledPackages = new List<FakePackage>();
		
		public bool IsPackageInstalled(IPackage package)
		{
			return FakeInstalledPackages.Contains(package);
		}
		
		public IQueryable<IPackage> GetPackages()
		{
			return FakeInstalledPackages.AsQueryable();
		}
		
		public void NoProjectsSelected()
		{
			FakeActiveProject = null;
			FakeActiveMSBuildProject = null;
		}
		
		public FakePackageManagementProject AddFakeProjectToReturnFromGetProject(string name)
		{
			var project = new FakePackageManagementProject(name);
			FakeProjectsToReturnFromGetProject.Add(name, project);
			return project;
		}
		
		public List<FakePackage> FakePackagesInReverseDependencyOrder = 
			new List<FakePackage>();
		
		public IEnumerable<IPackage> GetPackagesInReverseDependencyOrder()
		{
			return FakePackagesInReverseDependencyOrder;
		}
		
		public List<FakePackageManagementProject> FakeProjects =
			new List<FakePackageManagementProject>();
		
		public IPackageRepository SourceRepositoryPassedToGetProjects;
		
		public IEnumerable<IPackageManagementProject> GetProjects(IPackageRepository sourceRepository)
		{
			SourceRepositoryPassedToGetProjects = sourceRepository;
			return FakeProjects;
		}
		
		public FakePackageManagementProject AddFakeProject(string projectName)
		{
			var project = new FakePackageManagementProject(projectName);
			FakeProjects.Add(project);
			return project;
		}
		
		public FakePackage AddPackageToSharedLocalRepository(string packageId, string version)
		{
			var package = new FakePackage(packageId, version);
			FakeInstalledPackages.Add(package);
			return package;
		}
		
		public FakePackage AddPackageToSharedLocalRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			FakeInstalledPackages.Add(package);
			return package;
		}
		
		public string GetInstallPath(IPackage package)
		{
			throw new NotImplementedException();
		}
	}
}
