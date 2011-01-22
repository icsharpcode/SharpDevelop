// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageRepositoryPathsTests
	{
		PackageRepositoryPaths repositoryPaths;
		IProject testProject;
		PackageManagementOptions options;
		
		void CreatePackageRepositoryPaths()
		{
			repositoryPaths = new PackageRepositoryPaths(testProject, options);
		}
		
		void CreateTestProject()
		{
			testProject = ProjectHelper.CreateTestProject();
		}
		
		void CreatePackageManagementOptions()
		{
			options = new PackageManagementOptions(new Properties());
		}
		
		[Test]
		public void SolutionPackagesPath_ProjectAndSolutionHaveDifferentFolders_IsConfiguredPackagesFolderInsideSolutionFolder()
		{
			CreatePackageManagementOptions();
			CreateTestProject();
			testProject.ParentSolution.FileName = @"d:\projects\MyProject\MySolution.sln";
			options.PackagesDirectory = "MyPackages";
			CreatePackageRepositoryPaths();
			
			string expectedPath = @"d:\projects\MyProject\MyPackages";
			
			Assert.AreEqual(expectedPath, repositoryPaths.SolutionPackagesPath);
		}
	}
}
