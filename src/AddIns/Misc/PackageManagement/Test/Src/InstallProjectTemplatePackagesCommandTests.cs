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
using System.Windows.Input;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Templates;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class InstallProjectTemplatePackagesCommandTests
	{
		TestableInstallProjectTemplatePackagesCommand command;
		
		void CreateCommand()
		{
			command = new TestableInstallProjectTemplatePackagesCommand();
		}
		
		TestableProject CreateFakeProject(string name)
		{
			return ProjectHelper.CreateTestProject(name);
		}
		
		void RunCommandWithProjectCreateInfoAsOwner(TestableProject project)
		{
			var projects = new List<TestableProject>();
			projects.Add(project);
			RunCommandWithProjectCreateInfoAsOwner(projects);
		}
		
		void RunCommandWithProjectCreateInfoAsOwner(List<TestableProject> projects)
		{
			var createInfo = new ProjectTemplateResult(new ProjectTemplateOptions());
			createInfo.NewProjects.AddRange(projects);
			
			command.FakeProjectService.ProjectCollections.Add(new ImmutableModelCollection<IProject>(projects));
			
			RunCommandWithProjectCreateInfoAsOwner(createInfo);
		}
		
		void RunCommandWithProjectCreateInfoAsOwner(ProjectTemplateResult createInfo)
		{
			((ICommand)command).Execute(createInfo);
		}
		
		void RunCommandWithExceptionThrowingPackageReferences(Exception exception)
		{
			var exceptionThrowingPackageReferences = new ExceptionThrowingPackageReferencesForProject();
			exceptionThrowingPackageReferences.ExceptionToThrowOnInstall = exception;
			command.FakePackageReferencesForProject = exceptionThrowingPackageReferences;
			TestableProject project = CreateFakeProject("Test");
			RunCommandWithProjectCreateInfoAsOwner(project);
		}
		
		[Test]
		public void Run_OneProjectCreatedByNewProjectDialog_ProjectUsedToCreatePackageReferences()
		{
			CreateCommand();
			TestableProject expectedProject = CreateFakeProject("Test");
			RunCommandWithProjectCreateInfoAsOwner(expectedProject);
			
			MSBuildBasedProject project = command.ProjectPassedToCreatePackageReferencesForProject;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void Run_TwoProjectsCreatedByNewProjectDialog_TwoProjectsUsedToCreatePackageReferences()
		{
			CreateCommand();
			var expectedProjects = new List<TestableProject>();
			expectedProjects.Add(CreateFakeProject("Test1"));
			expectedProjects.Add(CreateFakeProject("Test2"));
			RunCommandWithProjectCreateInfoAsOwner(expectedProjects);
			
			List<MSBuildBasedProject> projects = command.ProjectsPassedToCreatePackageReferencesForProject;
			
			Assert.AreEqual(expectedProjects, projects);
		}
		
		[Test]
		public void Run_OneProjectCreatedByNewProjectDialog_PackageRepositoryCacheUsedToCreatePackageReferences()
		{
			CreateCommand();
			TestableProject project = CreateFakeProject("Test");
			RunCommandWithProjectCreateInfoAsOwner(project);
			
			IPackageRepositoryCache cache = command.PackageRepositoryCachePassedToCreatePackageReferencesForProject;
			IPackageRepositoryCache expectedCache = command.FakePackageRepositoryCache;
			
			Assert.AreEqual(expectedCache, cache);
		}
		
		[Test]
		public void Run_OneProjectCreatedByNewProjectDialog_PackagesAreInstalled()
		{
			CreateCommand();
			TestableProject project = CreateFakeProject("Test");
			RunCommandWithProjectCreateInfoAsOwner(project);
			
			bool installed = command.FakePackageReferencesForProject.IsInstallPackagesCalled;
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void Run_ExceptionThrownWhenInstallingPackages_ErrorMessageIsDisplayedToUser()
		{
			CreateCommand();
			var exception = new Exception("Test");
			RunCommandWithExceptionThrowingPackageReferences(exception);
			
			string errorMessageDisplayed = command.FakeMessageService.ErrorMessageDisplayed;
			
			Assert.AreEqual("Test", errorMessageDisplayed);
		}
		
		[Test]
		public void Run_ExceptionThrownWhenInstallingPackages_ExceptionIsLogged()
		{
			CreateCommand();
			var expectedException = new Exception("Test");
			RunCommandWithExceptionThrowingPackageReferences(expectedException);
			
			Exception exceptionLogged = command.FakeLoggingService.ExceptionLoggedAsError;
			
			Assert.AreEqual(expectedException, exceptionLogged);
		}
		
		[Test]
		public void Run_OneProjectCreatedByNewProjectDialog_ProjectReferencesAreRemoved()
		{
			CreateCommand();
			TestableProject project = CreateFakeProject("Test");
			RunCommandWithProjectCreateInfoAsOwner(project);
			
			bool removed = command.FakePackageReferencesForProject.IsRemovePackageReferencesCalled;
			
			Assert.IsTrue(removed);
		}
	}
}
