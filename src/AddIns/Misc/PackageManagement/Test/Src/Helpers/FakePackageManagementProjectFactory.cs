// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementProjectFactory : IPackageManagementProjectFactory
	{
		public List<FakePackageManagementProject> FakeProjectsCreated = 
			new List<FakePackageManagementProject>();
		
		public FakePackageManagementProject FirstFakeProjectCreated {
			get { return FakeProjectsCreated[0]; }
		}
		
		public IPackageRepository FirstRepositoryPassedToCreateProject {
			get { return RepositoriesPassedToCreateProject[0]; }
		}
		
		public List<IPackageRepository> RepositoriesPassedToCreateProject = 
			new List<IPackageRepository>();
		
		public MSBuildBasedProject FirstProjectPassedToCreateProject {
			get { return ProjectsPassedToCreateProject[0]; }
		}
		
		public List<MSBuildBasedProject> ProjectsPassedToCreateProject  =
			new List<MSBuildBasedProject>();
		
		public IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project)
		{
			RepositoriesPassedToCreateProject.Add(sourceRepository);
			ProjectsPassedToCreateProject.Add(project);
			
			var fakeProject = new FakePackageManagementProject();
			FakeProjectsCreated.Add(fakeProject);
			return fakeProject;
		}
	}
}
