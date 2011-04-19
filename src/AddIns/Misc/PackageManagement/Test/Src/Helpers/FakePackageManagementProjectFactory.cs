// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementProjectFactory : IPackageManagementProjectFactory
	{
		public FakePackageManagementProject FakeProject = new FakePackageManagementProject();
		public IPackageRepository RepositoryPassedToCreateProject;
		public MSBuildBasedProject ProjectPassedToCreateProject;
		
		public IPackageManagementProject CreateProject(IPackageRepository sourceRepository, MSBuildBasedProject project)
		{
			RepositoryPassedToCreateProject = sourceRepository;
			ProjectPassedToCreateProject = project;
			return FakeProject;
		}
	}
}
