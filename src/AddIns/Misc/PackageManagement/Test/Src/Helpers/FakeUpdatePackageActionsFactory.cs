// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
