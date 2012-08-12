// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.VisualStudio
{
	[TestFixture]
	public class FlavoredProjectTests
	{
		FlavoredProject project;
		TestableProject msbuildProject;
		
		void CreateFlavoredProject(MSBuildBasedProject msbuildProject)
		{
			project = new FlavoredProject(msbuildProject);
		}
		
		void CreateMSBuildProject(string fileName)
		{
			msbuildProject = ProjectHelper.CreateTestProject();
			msbuildProject.FileName = fileName;
		}
		
		void AddProjectTypeGuidsToMSBuildProject(string guids)
		{
			msbuildProject.SetProperty("ProjectTypeGuids", guids, false);
		}
		
		[Test]
		public void GetAggregateProjectTypeGuids_VisualBasicProject_ReturnsVisualBasicProjectTypeGuid()
		{
			CreateMSBuildProject(@"d:\projects\test\test.vbproj");
			CreateFlavoredProject(msbuildProject);
			
			string guids;
			int result = project.GetAggregateProjectTypeGuids(out guids);
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.AreEqual(ProjectTypeGuids.VBNet, guids);
		}
		
		[Test]
		public void GetAggregateProjectTypeGuids_UnknownProject_ReturnsEmptyString()
		{
			CreateMSBuildProject(@"d:\projects\test\test.unknown");
			CreateFlavoredProject(msbuildProject);
			
			string guids;
			int result = project.GetAggregateProjectTypeGuids(out guids);
			
			Assert.AreEqual(VsConstants.S_OK, result);
			Assert.AreEqual(String.Empty, guids);
		}
		
		[Test]
		public void GetAggregateProjectTypeGuids_MSBuildProjectHasProjectTypeGuidsDefined_ReturnsGuidsFromMSBuildProject()
		{
			CreateMSBuildProject(@"d:\projects\test\test.csproj");
			string expectedGuids = "{E53F8FEA-EAE0-44A6-8774-FFD645390401};{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}";
			AddProjectTypeGuidsToMSBuildProject(expectedGuids);
			CreateFlavoredProject(msbuildProject);
			
			string guids;
			project.GetAggregateProjectTypeGuids(out guids);
			
			Assert.AreEqual(expectedGuids, guids);
		}
	}
}
