// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Management.Automation;
using ICSharpCode.PackageManagement.EnvDTE;
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
			defaultProject = base.AddDefaultProjectToConsoleHost();
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		void EnableAllParameter()
		{
			cmdlet.All = new SwitchParameter(true);
		}
		
		void ProjectCollectionAssertAreEqual(IEnumerable<TestableProject> expectedProjects, IEnumerable<Project> actualProjects)
		{
			var expectedProjectNames = new List<string>();
			foreach (TestableProject testableProject in expectedProjects) {
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
			RunCmdlet();
						
			Assert.IsTrue(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
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
			
			fakeConsoleHost.AddFakeProject("A");
			fakeConsoleHost.AddFakeProject("B");
			
			RunCmdlet();
			
			var expectedProjects = fakeConsoleHost.FakeOpenProjects;
			var projects = fakeCommandRuntime.FirstObjectPassedToWriteObject as IEnumerable<Project>;
			
			ProjectCollectionAssertAreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void ProcessRecord_AllParameterSetAndConsoleHostHasTwoProjects_ProjectsWrittenToPipelineAreEnumerated()
		{
			CreateCmdlet();
			EnableAllParameter();
			
			fakeConsoleHost.AddFakeProject("A");
			fakeConsoleHost.AddFakeProject("B");
			
			RunCmdlet();
			
			bool enumerated = fakeCommandRuntime.EnumerateCollectionPassedToWriteObject;
			
			Assert.IsTrue(enumerated);
		}
		
		[Test]
		public void ProcessRecord_TwoProjectsOpenAndNameParameterMatchesOneProject_OneProjectWrittenToPipeline()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "B" };
			
			fakeConsoleHost.AddFakeProject("A");
			var projectB = fakeConsoleHost.AddFakeProject("B");
			
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
			
			fakeConsoleHost.AddFakeProject("A");
			var projectB = fakeConsoleHost.AddFakeProject("B");
			
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
			
			fakeConsoleHost.AddFakeProject("A");
			var projectB = fakeConsoleHost.AddFakeProject("B");
			var projectC = fakeConsoleHost.AddFakeProject("C");
			
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
			
			fakeConsoleHost.AddFakeProject("A");
			var projectB = fakeConsoleHost.AddFakeProject("B");
			var projectC = fakeConsoleHost.AddFakeProject("C");
			
			RunCmdlet();
			
			bool enumerated = fakeCommandRuntime.EnumerateCollectionPassedToWriteObject;
			
			Assert.IsTrue(enumerated);
		}
		
		[Test]
		public void ProcessRecord_ThreeProjectsAndTwoWildcardsUsedThatMatchTwoProjects_TwoProjectsWrittenToPipeline()
		{
			CreateCmdlet();
			cmdlet.Name = new string[] { "B*", "C*" };
			
			fakeConsoleHost.AddFakeProject("A");
			var projectB = fakeConsoleHost.AddFakeProject("B");
			var projectC = fakeConsoleHost.AddFakeProject("C");
			
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
