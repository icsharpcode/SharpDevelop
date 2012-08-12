// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		
		void CreateVsSolution()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			solution = new VsSolution(fakeProjectService);
		}
		
		int GetProjectOfUniqueName(string name)
		{
			return solution.GetProjectOfUniqueName(name, out hierarchy);
		}
		
		void AddProjectToMSBuildSolution(string fileName)
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			project.FileName = fileName;
			fakeProjectService.AddFakeProject(project);
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
			CreateVsSolution();
			AddProjectToMSBuildSolution(@"d:\projects\test\Test.csproj");
			
			int result = GetProjectOfUniqueName("Test.csproj");
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.IsNotNull(hierarchy);
		}
		
		[Test]
		public void GetProjectOfUniqueName_SolutionHasProjectButDoesNotMatchUniqueName_ReturnsError()
		{
			CreateVsSolution();
			AddProjectToMSBuildSolution(@"d:\projects\test\unknown.vbproj");
			
			int result = GetProjectOfUniqueName("Test.csproj");
			
			Assert.AreEqual(VsConstants.E_FAIL, result);
		}
		
		[Test]
		public void GetProjectOfUniqueName_UniqueNameCaseIsIgnored_ReturnsSuccess()
		{
			CreateVsSolution();
			AddProjectToMSBuildSolution(@"d:\projects\test\test.csproj");
			
			int result = GetProjectOfUniqueName("TEST.CSPROJ");
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.IsNotNull(hierarchy);
		}
		
		[Test]
		public void GetProjectOfUniqueName_ProjectTypeGuidsRetrievedFromAggregatableCSharpProject_ReturnsCSharpProjectTypeGuid()
		{
			CreateVsSolution();
			AddProjectToMSBuildSolution(@"d:\projects\test\test.csproj");
			
			GetProjectOfUniqueName("test.csproj");
			
			var project = hierarchy as IVsAggregatableProject;
			
			string guids;
			int result = project.GetAggregateProjectTypeGuids(out guids);
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.AreEqual(ProjectTypeGuids.CSharp, guids);
		}
	}
}
