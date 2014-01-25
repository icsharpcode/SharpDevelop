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
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;
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
			var solution = MockRepository.GenerateStrictMock<ISolution>();
			solution.Stub(s => s.Directory).Return(DirectoryName.Create(@"d:\projects\MyProject"));
			fakeProjectService.OpenSolution = solution;
			
			string directory = workingDirectory.GetWorkingDirectory();
			
			string expectedDirectory = @"'d:\projects\MyProject'";
			
			Assert.AreEqual(expectedDirectory, directory);
		}
	}
}
