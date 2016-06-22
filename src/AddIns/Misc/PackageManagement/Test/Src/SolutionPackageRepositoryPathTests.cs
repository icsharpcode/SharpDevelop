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
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SolutionPackageRepositoryPathTests
	{
		SolutionPackageRepositoryPath repositoryPath;
		IProject testProject;
		TestablePackageManagementOptions options;
		ISolution solution;
		FakeSettings settings;
		
		void CreateSolutionPackageRepositoryPath()
		{
			repositoryPath = new SolutionPackageRepositoryPath(testProject, options);
		}
		
		void CreateSolutionPackageRepositoryPath(ISolution solution)
		{
			repositoryPath = new SolutionPackageRepositoryPath(solution, options);
		}
		
		void CreateTestProject()
		{
			testProject = ProjectHelper.CreateTestProject();
		}
		
		void CreateSolution(string fileName)
		{
			solution = MockRepository.GenerateStrictMock<ISolution>();
			var file = FileName.Create(fileName);
			solution.Stub(s => s.FileName).Return(file);
			solution.Stub(s => s.Directory).Return(file.GetParentDirectory());
		}
		
		void CreateOptions()
		{
			options = new TestablePackageManagementOptions();
			settings = options.FakeSettings;
		}

		void SolutionNuGetConfigFileHasCustomPackagesPath(string fullPath)
		{
			settings.SetRepositoryPathSetting(fullPath);
		}
		
		[Test]
		public void PackageRepositoryPath_ProjectAndSolutionHaveDifferentFolders_IsConfiguredPackagesFolderInsideSolutionFolder()
		{
			CreateOptions();
			CreateTestProject();
			testProject.ParentSolution.Stub(s => s.Directory).Return(DirectoryName.Create(@"d:\projects\MyProject\"));
			options.PackagesDirectory = "MyPackages";
			CreateSolutionPackageRepositoryPath();
			
			string path = repositoryPath.PackageRepositoryPath;
			string expectedPath = @"d:\projects\MyProject\MyPackages";
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void PackageRepositoryPath_PassSolutionToConstructor_IsConfiguredPackagesFolderInsideSolutionFolder()
		{
			CreateOptions();
			CreateSolution(@"d:\projects\MySolution\MySolution.sln");
			options.PackagesDirectory = "Packages";
			CreateSolutionPackageRepositoryPath(solution);
			
			string path = repositoryPath.PackageRepositoryPath;
			string expectedPath = @"d:\projects\MySolution\Packages";
			
			Assert.AreEqual(expectedPath, path);
		}
		
		[Test]
		public void GetInstallPath_GetInstallPathForPackage_ReturnsPackagePathInsideSolutionPackagesRepository()
		{
			CreateOptions();
			CreateSolution(@"d:\projects\Test\MySolution\MyProject.sln");
			options.PackagesDirectory = "MyPackages";
			CreateSolutionPackageRepositoryPath(solution);
			
			var package = FakePackage.CreatePackageWithVersion("MyPackage", "1.2.1.40");
			
			string installPath = repositoryPath.GetInstallPath(package);
			
			string expectedInstallPath = 
				@"d:\projects\Test\MySolution\MyPackages\MyPackage.1.2.1.40";
			
			Assert.AreEqual(expectedInstallPath, installPath);
		}
		
		[Test]
		public void PackageRepositoryPath_SolutionHasNuGetFileThatOverridesDefaultPackagesRepositoryPath_OverriddenPathReturned()
		{
			CreateOptions();
			CreateSolution(@"d:\projects\MySolution\MySolution.sln");
			options.PackagesDirectory = "Packages";
			SolutionNuGetConfigFileHasCustomPackagesPath(@"d:\Team\MyPackages");
			CreateSolutionPackageRepositoryPath(solution);
			string expectedPath = @"d:\Team\MyPackages";

			string path = repositoryPath.PackageRepositoryPath;

			Assert.AreEqual(expectedPath, path);
		}
	}
}
