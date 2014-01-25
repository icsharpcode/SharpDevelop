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
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SelectedProjectsForUpdatedPackagesTests
	{
		TestableSelectedProjectsForUpdatedPackages selectedProjects;
		FakePackageManagementSolution fakeSolution;
		
		void CreateFakeSolution()
		{
			fakeSolution = new FakePackageManagementSolution();
		}
		
		void CreateSelectedProjects()
		{
			selectedProjects = new TestableSelectedProjectsForUpdatedPackages(fakeSolution);
		}
		
		List<IProject> AddSolutionWithTwoProjectsToProjectService(string projectName1, string projectName2)
		{
			ISolution solution = ProjectHelper.CreateSolution();
			TestableProject project1 = ProjectHelper.CreateTestProject(solution, projectName1);
			TestableProject project2 = ProjectHelper.CreateTestProject(solution, projectName2);
			
			fakeSolution.FakeMSBuildProjects.Add(project1);
			fakeSolution.FakeMSBuildProjects.Add(project2);
			
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName1);
			fakeSolution.AddFakeProjectToReturnFromGetProject(projectName2);
			
			return fakeSolution.FakeMSBuildProjects;
		}
		
		FakePackageManagementProject GetProject(string name)
		{
			return fakeSolution.FakeProjectsToReturnFromGetProject[name];
		}
		
		[Test]
		public void GetProjects_TwoProjectsAndPackageNotInstalledInAnyProject_IsEnabledIsFalseForAllSelectedProjects()
		{
			CreateFakeSolution();
			AddSolutionWithTwoProjectsToProjectService("Project A", "Project B");
			fakeSolution.NoProjectsSelected();
			CreateSelectedProjects();
			
			var fakePackage = new FakePackage("Test");
			List<IPackageManagementSelectedProject> projects =
				selectedProjects.GetProjects(fakePackage).ToList();
			
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(new FakeSelectedProject("Project A", selected: false, enabled: false));
			expectedProjects.Add(new FakeSelectedProject("Project B", selected: false, enabled: false));
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void GetProjects_TwoProjectsAndOlderPackageInstalledInFirstProject_IsEnabledIsTrueForFirstSelectedProjectButFalseForSecond()
		{
			CreateFakeSolution();
			AddSolutionWithTwoProjectsToProjectService("Project A", "Project B");
			fakeSolution.NoProjectsSelected();
			CreateSelectedProjects();
			
			var olderFakePackage = new FakePackage("Test", "1.0");
			FakePackageManagementProject projectA = GetProject("Project A");
			projectA.FakePackages.Add(olderFakePackage);
				
			var fakePackage = new FakePackage("Test", "1.2");
			List<IPackageManagementSelectedProject> projects =
				selectedProjects.GetProjects(fakePackage).ToList();
			
			var expectedProjects = new List<IPackageManagementSelectedProject>();
			expectedProjects.Add(new FakeSelectedProject("Project A", selected: true, enabled: true));
			expectedProjects.Add(new FakeSelectedProject("Project B", selected: false, enabled: false));
			
			SelectedProjectCollectionAssert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void IsProjectEnabled_PackageOlderThanPackageIntalledInProject_ReturnsFalse()
		{
			CreateFakeSolution();
			CreateSelectedProjects();
			
			var oldPackage = new FakePackage("Test", "1.0");
			var newPackage = new FakePackage("Test", "1.3");
			
			var project = new FakePackageManagementProject();
			project.FakePackages.Add(newPackage);
			
			bool enabled = selectedProjects.CallIsProjectEnabled(project, oldPackage);
			
			Assert.IsFalse(enabled);
		}
		
		[Test]
		public void IsProjectEnabled_PackageSameVersionAsPackageIntalledInProject_ReturnsFalse()
		{
			CreateFakeSolution();
			CreateSelectedProjects();
			
			var oldPackage = new FakePackage("Test", "1.0");
			var newPackage = new FakePackage("Test", "1.0");
			
			var project = new FakePackageManagementProject();
			project.FakePackages.Add(newPackage);
			
			bool enabled = selectedProjects.CallIsProjectEnabled(project, oldPackage);
			
			Assert.IsFalse(enabled);
		}
		
		[Test]
		public void IsProjectSelected_PackageSameVersionAsPackageIntalledInProject_ReturnsFalse()
		{
			CreateFakeSolution();
			CreateSelectedProjects();
			
			var oldPackage = new FakePackage("Test", "1.0");
			var newPackage = new FakePackage("Test", "1.0");
			
			var project = new FakePackageManagementProject();
			project.FakePackages.Add(newPackage);
			
			bool selected = selectedProjects.CallIsProjectSelected(project, oldPackage);
			
			Assert.IsFalse(selected);
		}
		
		[Test]
		public void IsProjectEnabled_PackageNewerThanPackageIntalledInProject_ReturnsTrue()
		{
			CreateFakeSolution();
			CreateSelectedProjects();
			
			var oldPackage = new FakePackage("Test", "1.0");
			var newPackage = new FakePackage("Test", "1.3");
			
			var project = new FakePackageManagementProject();
			project.FakePackages.Add(oldPackage);
			
			bool enabled = selectedProjects.CallIsProjectEnabled(project, newPackage);
			
			Assert.IsTrue(enabled);
		}
		
		[Test]
		public void IsProjectEnabled_PackageIdNotInstalledInProject_ReturnsFalse()
		{
			CreateFakeSolution();
			CreateSelectedProjects();
			
			var oldPackage = new FakePackage("Foo", "1.0");
			var newPackage = new FakePackage("Bar", "1.3");
			
			var project = new FakePackageManagementProject();
			project.FakePackages.Add(oldPackage);
			
			bool enabled = selectedProjects.CallIsProjectEnabled(project, newPackage);
			
			Assert.IsFalse(enabled);
		}
		
		[Test]
		public void IsProjectEnabled_PackageNewerThanPackageIntalledInProjectButPackageIdsDifferInCase_ReturnsTrue()
		{
			CreateFakeSolution();
			CreateSelectedProjects();
			
			var oldPackage = new FakePackage("test", "1.0");
			var newPackage = new FakePackage("TEST", "1.3");
			
			var project = new FakePackageManagementProject();
			project.FakePackages.Add(oldPackage);
			
			bool enabled = selectedProjects.CallIsProjectEnabled(project, newPackage);
			
			Assert.IsTrue(enabled);
		}
	}
}
