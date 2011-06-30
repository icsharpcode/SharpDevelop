// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		
		public virtual IPackageManagementProject GetActiveProject()
		{
			GetActiveProjectCallCount++;
			
			return FakeActiveProject;
		}
		
		public IPackageRepository RepositoryPassedToGetActiveProject;
		
		public virtual IPackageManagementProject GetActiveProject(IPackageRepository sourceRepository)
		{
			RepositoryPassedToGetActiveProject = sourceRepository;
			return FakeActiveProject;
		}
		
		public PackageSource PackageSourcePassedToGetProject;
		public string ProjectNamePassedToGetProject;
		
		public IPackageManagementProject GetProject(PackageSource source, string projectName)
		{
			PackageSourcePassedToGetProject = source;
			ProjectNamePassedToGetProject = projectName;
			return FakeActiveProject;
		}
		
		public IPackageRepository RepositoryPassedToGetProject;
		
		public IPackageManagementProject GetProject(IPackageRepository sourceRepository, string projectName)
		{
			RepositoryPassedToGetProject = sourceRepository;
			ProjectNamePassedToGetProject = projectName;
			return FakeActiveProject;
		}
		
		public List<IProject> FakeMSBuildProjects = new List<IProject>();
		
		public IEnumerable<IProject> GetMSBuildProjects()
		{
			return FakeMSBuildProjects;
		}
		
		public bool IsOpen { get; set; }
	}
}
