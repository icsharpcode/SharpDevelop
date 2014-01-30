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
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using Rhino.Mocks;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageManagementConsoleViewModelTests
	{
		TestablePackageManagementConsoleViewModel viewModel;
		FakePackageManagementProjectService projectService;
		FakePackageManagementConsoleHost consoleHost;
		List<string> propertiesChanged;
		
		void CreateConsoleHost()
		{
			consoleHost = new FakePackageManagementConsoleHost();
		}
		
		void CreateViewModel()
		{
			CreateConsoleHost();
			CreateViewModel(consoleHost);
		}
		
		void CreateViewModel(IPackageManagementConsoleHost consoleHost)
		{
			viewModel = new TestablePackageManagementConsoleViewModel(consoleHost);
			projectService = viewModel.FakeProjectService;
		}
		
		void CreateViewModel(IEnumerable<PackageSource> packageSources, IPackageManagementConsoleHost consoleHost)
		{
			viewModel = new TestablePackageManagementConsoleViewModel(packageSources, consoleHost);
			projectService = viewModel.FakeProjectService;
		}
		
		void CreateViewModel(IPackageManagementConsoleHost consoleHost, FakePackageManagementProjectService projectService)
		{
			viewModel = new TestablePackageManagementConsoleViewModel(consoleHost, projectService);
		}
		
		void CreateViewModelWithOneRegisteredPackageSource()
		{
			CreateConsoleHost();
			var packageSources = new List<PackageSource>();
			packageSources.Add(new PackageSource("First source"));
			CreateViewModel(packageSources, consoleHost);
		}
		
		void CreateViewModelWithTwoRegisteredPackageSources()
		{
			CreateConsoleHost();
			var packageSources = new List<PackageSource>();
			packageSources.Add(new PackageSource("First source"));
			packageSources.Add(new PackageSource("Second source"));
			CreateViewModel(packageSources, consoleHost);
		}
		
		PackageSource CreateViewModelWithTwoRegisteredPackageSourcesAndLastOneIsActivePackageSource()
		{
			CreateConsoleHost();
			var packageSources = new List<PackageSource>();
			packageSources.Add(new PackageSource("First source"));
			var activePackageSource = new PackageSource("Second source");
			packageSources.Add(activePackageSource);
			
			consoleHost.ActivePackageSource = activePackageSource;
			
			CreateViewModel(packageSources, consoleHost);
			
			return activePackageSource;
		}
		
		PackageSource CreateViewModelWithTwoRegisteredPackageSourcesAndFirstOneIsDisabledPackageSource()
		{
			CreateConsoleHost();
			var packageSources = new List<PackageSource>();
			var disabledPackageSource = new PackageSource("Disabled source") { IsEnabled = false };
			var enabledPackageSource = new PackageSource("Enabled source") { IsEnabled = true };
			packageSources.Add(disabledPackageSource);
			packageSources.Add(enabledPackageSource);
			CreateViewModel(packageSources, consoleHost);
			return enabledPackageSource;
		}
		
		ISolution CreateViewModelWithOneProjectOpen()
		{
			CreateConsoleHost();
			ISolution solution = CreateSolutionWithOneProject();
			projectService = new FakePackageManagementProjectService();
			projectService.OpenSolution = solution;
			projectService.ProjectCollections.Add(solution.Projects);
			CreateViewModel(consoleHost, projectService);
			
			return solution;
		}
		
		ISolution CreateSolutionWithOneProject()
		{
			TestableProject project = ProjectHelper.CreateTestProject();
			return project.ParentSolution;
		}
		
		PackageSource AddOnePackageSourceAndRemoveAnyExistingPackageSources()
		{
			viewModel.RegisteredPackageSources.Clear();
			return AddOnePackageSource();
		}
		
		PackageSource AddOnePackageSource()
		{
			return AddOnePackageSource("NewSource");
		}
		
		PackageSource AddOnePackageSource(string name)
		{
			var source = new PackageSource(name);
			viewModel.RegisteredPackageSources.Add(source);
			return source;
		}
		
		PackageSourceViewModel SelectSecondPackageSource()
		{
			PackageSourceViewModel selectedPackageSource = viewModel.PackageSources[1];
			viewModel.ActivePackageSource = selectedPackageSource;
			return selectedPackageSource;
		}

		void RecordPropertyChangedEvents()
		{
			propertiesChanged = new List<string>();
			viewModel.PropertyChanged += (sender, e) => propertiesChanged.Add(e.PropertyName);
		}
		
		ISolution CreateViewModelWithEmptySolutionOpen()
		{
			CreateConsoleHost();
			var solution = ProjectHelper.CreateSolution();
			projectService = new FakePackageManagementProjectService();
			projectService.OpenSolution = solution;
			projectService.ProjectCollections.Add(solution.Projects);
			CreateViewModel(consoleHost, projectService);
			return solution;
		}
		
		TestableProject AddProjectToSolution(ISolution solution)
		{
			var project = ProjectHelper.CreateTestProject(solution, "TestProject");
			return project;
		}
		
		void CloseSolution()
		{
			ISolution solution = projectService.OpenSolution;
			projectService.OpenSolution = null;
			projectService.ProjectCollections.Remove(solution.Projects);
			projectService.FireSolutionClosedEvent(solution);
		}
		
		void OpenSolution(ISolution solution)
		{
			projectService.OpenSolution = solution;
			projectService.ProjectCollections.Add(solution.Projects);
		}
		
		IProject RemoveProjectFromSolution(ISolution solution)
		{
			var project = solution.Projects.FirstOrDefault();
			((ICollection<IProject>)solution.Projects).Remove(project);
			return project;
		}
		
		[Test]
		public void PackageSources_OneRegisteredPackageSourceWhenConsoleCreated_OnePackageSourceDisplayed()
		{
			CreateViewModelWithOneRegisteredPackageSource();
			
			var expectedPackageSources = viewModel.RegisteredPackageSources;
			var actualPackageSources = viewModel.PackageSources;
			
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
		
		[Test]
		public void ActivePackageSource_OneRegisteredPackageSourceWhenConsoleCreated_SinglePackageSourceIsActivePackageSource()
		{
			CreateViewModelWithOneRegisteredPackageSource();
			var expectedPackageSource = viewModel.RegisteredPackageSources[0];
			var actualPackageSource = viewModel.ActivePackageSource.GetPackageSource();
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ActivePackageSource_TwoRegisteredPackageSourcesAndLastOneIsActiveWhenConsoleCreated_SecondPackageSourceIsActiveInViewModel()
		{
			var expectedPackageSource = CreateViewModelWithTwoRegisteredPackageSourcesAndLastOneIsActivePackageSource();
			
			var actualPackageSource = viewModel.ActivePackageSource.GetPackageSource();
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void PackageSources_OriginalPackageSourceRemovedAndOnePackageSourceAddedAfterConsoleCreated_NewPackageSourceIsDisplayed()
		{
			CreateViewModel();
			AddOnePackageSourceAndRemoveAnyExistingPackageSources();
			
			var expectedPackageSources = viewModel.RegisteredPackageSources;
			var actualPackageSources = viewModel.PackageSources;
			
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
		
		[Test]
		public void PackageSources_TwoRegisteredPackageSourcesWhenConsoleCreated_ThreePackageSourcesReturnedIncludingAggregatePackageSource()
		{
			CreateViewModelWithTwoRegisteredPackageSources();
			
			var actualPackageSources = viewModel.PackageSources;
			
			var expectedPackageSources = new List<PackageSource>(viewModel.RegisteredPackageSources);
			expectedPackageSources.Add(RegisteredPackageSourceSettings.AggregatePackageSource);
			
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
		
		[Test]
		public void ActivePackageSource_OriginalPackageSourceRemovedAndOnePackageSourceAddedAfterConsoleCreated_ActivePackageSourceIsUpdatedToNewPackageSource()
		{
			CreateViewModel();
			AddOnePackageSourceAndRemoveAnyExistingPackageSources();
			
			var expectedPackageSource = viewModel.RegisteredPackageSources[0];
			var actualPackageSource = viewModel.ActivePackageSource.GetPackageSource();
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ActivePackageSource_OriginalPackageSourceRemovedAndOnePackageSourceAddedAfterConsoleCreated_PropertyChangedEventFiredForActivePackageSource()
		{
			CreateViewModel();
			RecordPropertyChangedEvents();
			AddOnePackageSourceAndRemoveAnyExistingPackageSources();
			
			bool result = propertiesChanged.Contains("ActivePackageSource");
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ActivePackageSource_TwoPackageSourcesAndActivePackageSourceChangedToSecondOne_ActivePackageSourceChoiceIsRemembered()
		{
			CreateViewModel();
			AddOnePackageSource();
			var selectedPackageSource = SelectSecondPackageSource();
			
			var actualPackageSource = viewModel.ActivePackageSource;
			
			Assert.AreEqual(selectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ActivePackageSource_SelectedPackageSourceIsRemoved_ActivePackageSourceIsChangedToFirstPackageSource()
		{
			CreateViewModel();
			AddOnePackageSource();
			SelectSecondPackageSource();
			viewModel.RegisteredPackageSources.RemoveAt(1);
			
			var expectedPackageSource = viewModel.PackageSources[0];
			var actualPackageSource = viewModel.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void Projects_OneProjectOpenWhenConsoleCreated_OpenProjectIsInProjectsCollection()
		{
			var solution = CreateViewModelWithOneProjectOpen();
			var expectedProjects = solution.Projects;
			var actualProjects = viewModel.Projects;
			
			CollectionAssert.AreEqual(expectedProjects, actualProjects);
		}
		
		[Test]
		public void DefaultProject_OneProjectOpenWhenConsoleCreated_OpenProjectIsDefaultProject()
		{
			var solution = CreateViewModelWithOneProjectOpen();
			var expectedProject = solution.Projects.First();
			var actualProject = viewModel.DefaultProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void Constructor_EmptySolutionOpenWhenConsoleViewModelCreated_DoesNotThrowException()
		{
			CreateConsoleHost();
			projectService = new FakePackageManagementProjectService();
			var solution = ProjectHelper.CreateSolution();
			projectService.OpenSolution = solution;

			Assert.DoesNotThrow(() => CreateViewModel(consoleHost, projectService));
		}
		
		[Test]
		public void Projects_ProjectAddedToEmptySolution_ProjectDisplayed()
		{
			var solution = CreateViewModelWithEmptySolutionOpen();
			var project = AddProjectToSolution(solution);
			
			var actualProjects = viewModel.Projects;
			var expectedProjects = solution.Projects;
			
			CollectionAssert.AreEqual(expectedProjects, actualProjects);
		}
		
		[Test]
		public void DefaultProject_ProjectAddedToEmptySolution_ProjectAddedIsDefaultProject()
		{
			var solution = CreateViewModelWithEmptySolutionOpen();
			var project = AddProjectToSolution(solution);
			
			var actualProject = viewModel.DefaultProject;
			
			Assert.AreEqual(project, actualProject);
		}
		
		[Test]
		public void Projects_SolutionClosed_ProjectsRemovedFromList()
		{
			CreateViewModelWithOneProjectOpen();
			CloseSolution();
			
			int count = viewModel.Projects.Count;
			
			Assert.AreEqual(0, count);
		}
		
		[Test]
		public void DefaultProject_SolutionClosed_DefaultProjectIsSetToNull()
		{
			CreateViewModelWithOneProjectOpen();
			CloseSolution();
			
			Assert.IsNull(viewModel.DefaultProject);
		}
		
		[Test]
		public void DefaultProject_SolutionClosed_PropertyChangedEventFiredForDefaultProject()
		{
			CreateViewModelWithOneProjectOpen();
			RecordPropertyChangedEvents();
			CloseSolution();
			
			bool result = propertiesChanged.Contains("DefaultProject");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Projects_SolutionWithOneProjectLoaded_ProjectsListUpdated()
		{
			CreateViewModel();
			var solution = CreateSolutionWithOneProject();
			OpenSolution(solution);
			
			var actualProjects = viewModel.Projects;
			var expectedProjects = solution.Projects;
			
			CollectionAssert.AreEqual(expectedProjects, actualProjects);
		}
		
		[Test]
		public void DefaultProject_SolutionWithOneProjectLoaded_DefaultProjectIsSetToProjectInSolution()
		{
			CreateViewModel();
			var solution = CreateSolutionWithOneProject();
			OpenSolution(solution);
			
			var actualProject = viewModel.DefaultProject;
			var expectedProject = viewModel.Projects.FirstOrDefault();
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void DefaultProject_SolutionWithOneProjectLoaded_PropertyChangedEventFiredForDefaultProjectI()
		{
			CreateViewModel();
			var solution = CreateSolutionWithOneProject();
			RecordPropertyChangedEvents();
			OpenSolution(solution);
			
			bool result = propertiesChanged.Contains("DefaultProject");
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Projects_ProjectRemovedFromSolution_ProjectRemovedFromList()
		{
			var solution = CreateViewModelWithOneProjectOpen();
			var project = RemoveProjectFromSolution(solution);
			
			var actualProjects = viewModel.Projects;
			var expectedProjects = solution.Projects;
			
			CollectionAssert.AreEqual(expectedProjects, actualProjects);
		}
		
		[Test]
		public void DefaultProject_ProjectRemovedFromSolution_DefaultProjectIsUpdated()
		{
			var solution = CreateViewModelWithOneProjectOpen();
			var project = RemoveProjectFromSolution(solution);
			
			var actualProject = viewModel.DefaultProject;
			
			Assert.IsNull(actualProject);
		}
		
		[Test]
		public void DefaultProject_OneProjectOpenWhenConsoleCreated_DefaultProjectSetForConsole()
		{
			var solution = CreateViewModelWithOneProjectOpen();
			var expectedProject = solution.Projects.First();
			var actualProject = consoleHost.DefaultProject;
			
			Assert.AreEqual(expectedProject, actualProject);
		}
		
		[Test]
		public void ActivePackageSource_OneRegisteredPackageSourceWhenConsoleCreated_ActivePackageSourceSetForConsole()
		{
			CreateViewModelWithOneRegisteredPackageSource();
			var expectedPackageSource = viewModel.RegisteredPackageSources[0];
			var actualPackageSource = consoleHost.ActivePackageSource;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ShutdownConsole_ViewModelShuttingDown_ConsoleIsDisposed()
		{
			CreateViewModel();
			viewModel.ShutdownConsole();
			
			Assert.IsTrue(consoleHost.IsDisposeCalled);
		}
		
		[Test]
		public void ClearConsoleCommand_Executed_ClearsConsole()
		{
			CreateViewModel();
			viewModel.ClearConsoleCommand.Execute(null);
			
			Assert.IsTrue(consoleHost.IsClearCalled);
		}
		
		[Test]
		public void ClearConsole_CommandExecuted_WritesPrompt()
		{
			CreateViewModel();
			viewModel.ClearConsole();
			
			Assert.IsTrue(consoleHost.IsWritePromptCalled);
		}
		
		[Test]
		public void Constructor_NewViewModelCreated_ConsoleSetOnConsoleHost()
		{
			CreateViewModel();
			
			Assert.AreEqual(viewModel.FakeConsole, consoleHost.ScriptingConsole);
		}
		
		[Test]
		public void Constructor_NewViewModelCreated_ConsoleHostIsRun()
		{
			CreateViewModel();
			
			Assert.IsTrue(consoleHost.IsRunCalled);
		}
		
		[Test]
		public void ShutdownConsole_NewViewModelCreated_ScriptingConsoleIsShutdown()
		{
			CreateViewModel();
			viewModel.ShutdownConsole();
			
			bool shutdownConsole = consoleHost.IsShutdownConsoleCalled;
			Assert.IsTrue(shutdownConsole);
		}
		
		[Test]
		public void ShutdownConsole_ConsoleHostIsRunningIsTrue_ReturnsFalse()
		{
			CreateViewModel();
			consoleHost.IsRunning = true;
			bool shutdownConsole = viewModel.ShutdownConsole();
			
			Assert.IsFalse(shutdownConsole);
		}
		
		[Test]
		public void ShutdownConsole_ConsoleHostIsRunningIsFalse_ReturnsTrue()
		{
			CreateViewModel();
			consoleHost.IsRunning = false;
			bool shutdownConsole = viewModel.ShutdownConsole();
			
			Assert.IsTrue(shutdownConsole);
		}
		
		[Test]
		public void ActivePackageSource_SetToNullWhenPackageSourcesCleared_DoesNotThrowNullReferenceException()
		{
			CreateViewModel();
			Assert.DoesNotThrow(() => viewModel.ActivePackageSource = null);
		}
		
		[Test]
		public void PackageSources_TwoRegisteredPackageSourcesButOnlyOneEnabledWhenConsoleCreated_OnlyEnabledPackageSourceDisplayed()
		{
			PackageSource enabledPackageSource = 
				CreateViewModelWithTwoRegisteredPackageSourcesAndFirstOneIsDisabledPackageSource();
			
			ObservableCollection<PackageSourceViewModel> actualPackageSources = viewModel.PackageSources;
			
			var expectedPackageSources = new List<PackageSource>();
			expectedPackageSources.Add(enabledPackageSource);
			PackageSourceCollectionAssert.AreEqual(expectedPackageSources, actualPackageSources);
		}
	}
}
