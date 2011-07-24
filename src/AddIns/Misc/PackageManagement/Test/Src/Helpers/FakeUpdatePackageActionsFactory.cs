// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeUpdatePackageActionsFactory : IUpdatePackageActionsFactory
	{
		public FakeUpdatePackageActions FakeUpdateAllPackagesInProject = 
			new FakeUpdatePackageActions();
		
		public IPackageManagementProject ProjectPassedToCreateUpdateAllPackagesInProject;
		
		public IUpdatePackageActions CreateUpdateAllPackagesInProject(IPackageManagementProject project)
		{
			ProjectPassedToCreateUpdateAllPackagesInProject = project;
			return FakeUpdateAllPackagesInProject;
		}
		
		public FakeUpdatePackageActions FakeUpdateAllPackagesInSolution = 
			new FakeUpdatePackageActions();
		
		public IPackageManagementSolution SolutionPassedToCreateUpdateAllPackagesInSolution;
		public IPackageRepository SourceRepositoryPassedToCreateUpdateAllPackagesInSolution;
		
		public IUpdatePackageActions CreateUpdateAllPackagesInSolution(
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository)
		{
			SolutionPassedToCreateUpdateAllPackagesInSolution = solution;
			SourceRepositoryPassedToCreateUpdateAllPackagesInSolution = sourceRepository;
			return FakeUpdateAllPackagesInSolution;
		}
		
		public FakeUpdatePackageActions FakeUpdatePackageInAllProjects = 
			new FakeUpdatePackageActions();
		
		public IPackageManagementSolution SolutionPassedToCreateUpdatePackageInAllProjects;
		public IPackageRepository SourceRepositoryPassedToCreateUpdatePackageInAllProjects;
		public PackageReference PackageReferencePassedToCreateUpdatePackageInAllProjects;
		
		 public IUpdatePackageActions CreateUpdatePackageInAllProjects(
			PackageReference packageReference,
			IPackageManagementSolution solution,
			IPackageRepository sourceRepository)
		{
			PackageReferencePassedToCreateUpdatePackageInAllProjects = packageReference;
			SolutionPassedToCreateUpdatePackageInAllProjects = solution;
			SourceRepositoryPassedToCreateUpdatePackageInAllProjects = sourceRepository;
			return FakeUpdatePackageInAllProjects;
		}
	}
}
