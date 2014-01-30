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
using ICSharpCode.PackageManagement.VisualStudio;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class VsSolutionTests
	{
		VsSolution solution;
		IVsHierarchy hierarchy;
		FakePackageManagementProjectService fakeProjectService;
		
		void CreateVsSolution(string solutionFileName = @"d:\projects\test\Test.sln")
		{
			var helper = new SolutionHelper(solutionFileName);
			fakeProjectService = new FakePackageManagementProjectService();
			fakeProjectService.OpenSolution = helper.MSBuildSolution;
			solution = new VsSolution(fakeProjectService);
		}
		
		int GetProjectOfUniqueName(string name)
		{
			return solution.GetProjectOfUniqueName(name, out hierarchy);
		}
		
		void AddProjectToMSBuildSolution(string fileName)
		{
			TestableProject project = ProjectHelper.CreateTestProject(fakeProjectService.OpenSolution, "Test", fileName);
			project.SetProperty("ProjectTypeGuids", null);
			fakeProjectService.AddProject(project);
		}
		
		[Test]
		public void GetProjectOfUniqueName_NoSolutionOpen_ReturnsError()
		{
			CreateVsSolution();
			
			int result = GetProjectOfUniqueName("Test.csproj");
			
			Assert.AreEqual(VsConstants.E_FAIL, result);
		}
		
		[Test]
		public void GetProjectOfUniqueName_SolutionHasProjectMatchingUniqueName_ReturnsSuccessAndVsHierarchy()
		{
			CreateVsSolution(@"d:\projects\test\Test.sln");
			AddProjectToMSBuildSolution(@"d:\projects\test\Test.csproj");
			
			int result = GetProjectOfUniqueName("Test.csproj");
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.IsNotNull(hierarchy);
		}
		
		[Test]
		public void GetProjectOfUniqueName_SolutionHasProjectButDoesNotMatchUniqueName_ReturnsError()
		{
			CreateVsSolution(@"d:\projects\test\Test.sln");
			AddProjectToMSBuildSolution(@"d:\projects\test\unknown.vbproj");
			
			int result = GetProjectOfUniqueName("Test.csproj");
			
			Assert.AreEqual(VsConstants.E_FAIL, result);
		}
		
		[Test]
		public void GetProjectOfUniqueName_UniqueNameCaseIsIgnored_ReturnsSuccess()
		{
			CreateVsSolution(@"d:\projects\test\Test.sln");
			AddProjectToMSBuildSolution(@"d:\projects\test\test.csproj");
			
			int result = GetProjectOfUniqueName("TEST.CSPROJ");
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.IsNotNull(hierarchy);
		}
		
		[Test]
		public void GetProjectOfUniqueName_ProjectTypeGuidsRetrievedFromAggregatableCSharpProject_ReturnsCSharpProjectTypeGuid()
		{
			CreateVsSolution(@"d:\projects\test\Test.sln");
			AddProjectToMSBuildSolution(@"d:\projects\test\test.csproj");
			
			GetProjectOfUniqueName("test.csproj");
			
			var project = hierarchy as IVsAggregatableProject;
			
			string guids;
			int result = project.GetAggregateProjectTypeGuids(out guids);
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.AreEqual(ProjectTypeGuids.CSharp.ToString(), guids);
		}
		
		[Test]
		public void GetProjectOfUniqueName_UniqueNameIncludesProjectFolderInsideSolutionr_ReturnsSuccess()
		{
			CreateVsSolution(@"d:\projects\test\Test.sln");
			AddProjectToMSBuildSolution(@"d:\projects\test\Project\MyProject.csproj");
			
			int result = GetProjectOfUniqueName(@"Project\MyProject.csproj");
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.IsNotNull(hierarchy);
		}
	}
}
