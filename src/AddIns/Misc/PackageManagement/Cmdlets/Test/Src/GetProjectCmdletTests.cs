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
using System.Collections.Generic;
using System.Management.Automation;

using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class GetProjectCmdletTests : CmdletTestsBase
	{
		TestableGetProjectCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		TestableProject defaultProject;
		FakeCommandRuntime fakeCommandRuntime;
		FakePackageManagementSolution fakeSolution;
		
		void CreateCmdletWithoutActiveProject()
		{
			cmdlet = new TestableGetProjectCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
		}
		
		void CreateCmdlet()
		{
			cmdlet = new TestableGetProjectCmdlet();
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			fakeCommandRuntime = cmdlet.FakeCommandRuntime;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeSolution = cmdlet.FakeSolution;
			defaultProject = base.AddDefaultProjectToConsoleHost();
		}
		
		TestableProject AddFakeProject(string name)
		{
			var project = ProjectHelper.CreateTestProject(name);
			fakeSolution.FakeMSBuildProjects.Add(project);
			return project;
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		void EnableAllParameter()
		{
			cmdlet.All = new SwitchParameter(true);
		}
		
		void ProjectCollectionAssertAreEqual(IEnumerable<IProject> expectedProjects, IEnumerable<Project> actualProjects)
		{
			var expectedProjectNames = new List<string>();
			foreach (IProject testableProject in expectedProjects) {
				expectedProjectNames.Add(testableProject.Name);
			}
			
			var actualProjectNames = new List<string>();
			foreach (Project project in actualProjects) {
				actualProjectNames.Add(project.Name);
			}
			
			CollectionAssert.AreEqual(expectedProjectNames, actualProjectNames);
		}
		
		[Test]
		public void ProcessRecord_NoActiveProject_ThrowsNoProjectOpenTerminatingError()
		{
			CreateCmdletWithoutActiveProject();
			
			Assert.Throws(typeof(FakeCmdletTerminatingErrorException), () => RunCmdlet());
		}
		
		[Test]
		public void ProcessRecord_HostHasDefaultProject_DTEProjectWrittenToPipeline()
		{
			CreateCmdlet();
			defaultProject.Name = "MyProject";
			RunCmdlet();
			
			Project project = fakeCommandRuntime.FirstObjectPassedToWriteObject as Project;
			string projectName = project.Name;
			
			Assert.AreEqual("MyProject", projectName);
		}
		
		[Test]
		public void ProcessRecord_AllParameterSetAndConsoleHostHasTwoProjects_TwoProjectsWrittenToPipeline()
		{
			CreateCmdlet();
			EnableAllParameter();
			
			AddFakeProject("A");
			AddFakeProject("B");
			
			RunCmdlet();
			
			var expectedProjects = fakeSolution.FakeMSBuildProjects;
			var projects = fakeCommandRuntime.FirstObjectPassedToWriteObject as IEnumerable<Project>;
			
			ProjectCollectionAssertAreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void ProcessRecord_AllParameterSetAndConsoleHostHasTwoProjects_ProjectsWrittenToPipelineAreEnumerated()
		{
			CreateCmdlet();
			EnableAllParameter();
			
			AddFakeProject("A");
			AddFakeProject("B");
			
			RunCmdlet();
			
			bool enumerated = fakeCommandRuntime.EnumerateCollectionPassedToWriteObject;
			
			Assert.IsTrue(enumerated);
		}
		
		[Test]
		public void ProcessRecord_TwoProjectsOpenAndNameParameterMatchesOneProject_OneProjectWrittenToPipeline()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "B" };
			
			AddFakeProject("A");
			var projectB = AddFakeProject("B");
			
			RunCmdlet();
			
			var projects = fakeCommandRuntime.FirstObjectPassedToWriteObject as IEnumerable<Project>;
			var expectedProjects = new TestableProject[] {
				projectB
			};
			
			ProjectCollectionAssertAreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void ProcessRecord_TwoProjectsOpenAndNameParameterMatchesOneProjectButWithDifferentCase_OneProjectWrittenToPipeline()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "b" };
			
			AddFakeProject("A");
			var projectB = AddFakeProject("B");
			
			RunCmdlet();
			
			var projects = fakeCommandRuntime.FirstObjectPassedToWriteObject as IEnumerable<Project>;
			var expectedProjects = new TestableProject[] {
				projectB
			};
			
			ProjectCollectionAssertAreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void ProcessRecord_ThreeProjectsOpenAndTwoNameParametersMatchesTwoProjects_TwoProjectsWrittenToPipeline()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "B", "C" };
			
			AddFakeProject("A");
			var projectB = AddFakeProject("B");
			var projectC = AddFakeProject("C");
			
			RunCmdlet();
			
			var projects = fakeCommandRuntime.FirstObjectPassedToWriteObject as IEnumerable<Project>;
			var expectedProjects = new TestableProject[] {
				projectB,
				projectC
			};
			
			ProjectCollectionAssertAreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void ProcessRecord_ThreeProjectsOpenAndTwoNameParametersMatchesTwoProjects_ProjectsWrittenToPipelineAreEnumerated()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "B", "C" };
			
			AddFakeProject("A");
			var projectB = AddFakeProject("B");
			var projectC = AddFakeProject("C");
			
			RunCmdlet();
			
			bool enumerated = fakeCommandRuntime.EnumerateCollectionPassedToWriteObject;
			
			Assert.IsTrue(enumerated);
		}
		
		[Test]
		public void ProcessRecord_ThreeProjectsAndTwoWildcardsUsedThatMatchTwoProjects_TwoProjectsWrittenToPipeline()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "B*", "C*" };
			
			AddFakeProject("A");
			var projectB = AddFakeProject("B");
			var projectC = AddFakeProject("C");
			
			RunCmdlet();
			
			var projects = fakeCommandRuntime.FirstObjectPassedToWriteObject as IEnumerable<Project>;
			var expectedProjects = new TestableProject[] {
				projectB,
				projectC
			};
			
			ProjectCollectionAssertAreEqual(expectedProjects, projects);
		}
	}
}
