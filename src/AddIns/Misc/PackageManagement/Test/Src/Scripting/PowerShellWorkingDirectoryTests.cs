// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PowerShellWorkingDirectoryTests
	{
		FakePackageManagementProjectService fakeProjectService;
		PowerShellWorkingDirectory workingDirectory;
		
		void CreateWorkingDirectory()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			workingDirectory = new PowerShellWorkingDirectory(fakeProjectService);
		}
		
		[Test]
		public void GetWorkingDirectory_NoSolutionOpen_ReturnsUserProfileFolder()
		{
			CreateWorkingDirectory();
			fakeProjectService.OpenSolution = null;
			
			string directory = workingDirectory.GetWorkingDirectory();
			
			string expectedDirectory = "$env:USERPROFILE";
			
			Assert.AreEqual(expectedDirectory, directory);
		}
		
		[Test]
		public void GetWorkingDirectory_SolutionOpen_ReturnsSolutionDirectory()
		{
			CreateWorkingDirectory();
			var solution = new Solution(new MockProjectChangeWatcher());
			solution.FileName = @"d:\projects\MyProject\myproject.sln";
			fakeProjectService.OpenSolution = solution;
			
			string directory = workingDirectory.GetWorkingDirectory();
			
			string expectedDirectory = @"'d:\projects\MyProject'";
			
			Assert.AreEqual(expectedDirectory, directory);
		}
	}
}
