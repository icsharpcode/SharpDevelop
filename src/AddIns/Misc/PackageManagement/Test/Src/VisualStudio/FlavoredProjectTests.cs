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
			msbuildProject.FileName = new FileName(fileName);
			msbuildProject.SetProperty("ProjectTypeGuids", null);
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
			Assert.AreEqual(ProjectTypeGuids.VB.ToString(), guids);
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
