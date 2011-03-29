// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class GetPackageCmdletTests : PackageManagementCmdletTests
	{
		TestableGetPackageCmdlet cmdlet;
		FakePackageManagementService fakePackageManagementService;
		FakeCommandRuntime fakeCommandRuntime;
		FakeCmdletTerminatingError fakeTerminatingError;
		
		void CreateCmdlet()
		{
			cmdlet = new TestableGetPackageCmdlet();
			fakePackageManagementService = cmdlet.FakePackageManagementService;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeCommandRuntime = cmdlet.FakeCommandRuntime;
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
		}
		
		void RunCmdlet()
		{
			cmdlet.RunProcessRecord();
		}
		
		void EnableListAvailableParameter()
		{
			cmdlet.ListAvailable = new SwitchParameter(true);
		}
		
		void EnableUpdatesParameter()
		{
			cmdlet.Updates = new SwitchParameter(true);
		}
		
		FakePackage AddPackageToProjectManagerLocalRepository(string version)
		{
			return AddPackageToProjectManagerLocalRepository("Test", version);
		}
		
		FakePackage AddPackageToProjectManagerLocalRepository(string id, string version)
		{
			var package = FakePackage.CreatePackageWithVersion(id, version);
			fakePackageManagementService
				.FakeProjectManagerToReturnFromCreateProjectManager
				.FakeLocalRepository
				.FakePackages.Add(package);
			return package;
		}
		
		FakePackage AddPackageToAggregateRepository(string version)
		{
			return AddPackageToAggregateRepository("Test", version);
		}
		
		FakePackage AddPackageToAggregateRepository(string id, string version)
		{
			return fakePackageManagementService.AddFakePackageWithVersionToAggregrateRepository(id, version);
		}
		
		void SetFilterParameter(string filter)
		{
			cmdlet.Filter = filter;
		}
		
		void SetSourceParameter(string source)
		{
			cmdlet.Source = source;
		}
		
		void EnableRecentParameter()
		{
			cmdlet.Recent = new SwitchParameter(true);
		}
		
		void SetSkipParameter(int skip)
		{
			cmdlet.Skip = skip;
		}
		
		void SetTakeParameter(int take)
		{
			cmdlet.Take = take;
		}

		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasOnePackage_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			repository.AddOneFakePackage("Test");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = repository.FakePackages;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasThreePackages_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			repository.AddOneFakePackage("A");
			repository.AddOneFakePackage("B");
			repository.AddOneFakePackage("C");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = repository.FakePackages;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasTwoPackages_PackagesAreSortedById()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			var packageB = repository.AddOneFakePackage("B");
			var packageA = repository.AddOneFakePackage("A");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				packageA,
				packageB
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenNoPackageSourceSpecified_PackageSourceTakenFromConsoleHost()
		{
			CreateCmdlet();
			var source = AddPackageSourceToConsoleHost();
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualSource = fakePackageManagementService.PackageSourcePassedToCreatePackageRepository;
			
			Assert.AreEqual(source, actualSource);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_ReturnsPackagesInstalledForProjectSelectedInConsole()
		{
			CreateCmdlet();
			AddDefaultProjectToConsoleHost();
			FakeProjectManager projectManager = fakePackageManagementService.FakeProjectManagerToReturnFromCreateProjectManager;
			projectManager.FakeLocalRepository.AddOneFakePackage("One");
			projectManager.FakeLocalRepository.AddOneFakePackage("Two");
			
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = projectManager.FakeLocalRepository.FakePackages;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_SelectedActivePackageSourceInConsoleHostUsedToCreateRepository()
		{
			CreateCmdlet();
			var source = AddPackageSourceToConsoleHost();
			
			RunCmdlet();
			
			var actualSource = fakePackageManagementService.PackageSourcePassedToCreatePackageRepository;
			
			Assert.AreEqual(source, actualSource);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_SelectedActiveRepositoryInConsoleHostUsedToCreateProjectManager()
		{
			CreateCmdlet();
			AddPackageSourceToConsoleHost();
			
			RunCmdlet();
			
			var actualRepository = fakePackageManagementService.PackageRepositoryPassedToCreateProjectManager;
			var expectedRepository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_DefaultProjectInConsoleHostUsedToCreateProjectManager()
		{
			CreateCmdlet();
			AddPackageSourceToConsoleHost();
			var project = AddDefaultProjectToConsoleHost();
			
			RunCmdlet();
			
			var actualProject = fakePackageManagementService.ProjectPassedToCreateProjectManager;
			
			Assert.AreEqual(project, actualProject);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequested_ReturnsUpdatedPackagesForActiveProject()
		{
			CreateCmdlet();
			AddDefaultProjectToConsoleHost();
			AddPackageToProjectManagerLocalRepository("1.0.0.0");
			var updatedPackage = AddPackageToAggregateRepository("1.1.0.0");
			
			EnableUpdatesParameter();
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequested_ActiveProjectWhenCreatingProjectManager()
		{
			CreateCmdlet();
			var project = AddDefaultProjectToConsoleHost();
			EnableUpdatesParameter();
			RunCmdlet();
			
			var actualProject = fakePackageManagementService.ProjectPassedToCreateProjectManager;
			
			Assert.AreEqual(project, actualProject);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequested_AggregateRepositoryUsedWhenCreatingProjectManager()
		{
			CreateCmdlet();
			AddDefaultProjectToConsoleHost();
			EnableUpdatesParameter();
			RunCmdlet();
			
			var actualRepository = fakePackageManagementService.PackageRepositoryPassedToCreateProjectManager;
			var expectedRepository = fakePackageManagementService.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesAndFilterResults_PackagesReturnedMatchFilter()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			repository.AddOneFakePackage("A");
			var package = repository.AddOneFakePackage("B");
			repository.AddOneFakePackage("C");
			
			EnableListAvailableParameter();
			SetFilterParameter("B");
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				 package
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_FilterParameterPassed_InstallPackagesAreFiltered()
		{
			CreateCmdlet();
			AddPackageSourceToConsoleHost();
			FakeProjectManager projectManager = fakePackageManagementService.FakeProjectManagerToReturnFromCreateProjectManager;
			projectManager.FakeLocalRepository.AddOneFakePackage("A");
			var package = projectManager.FakeLocalRepository.AddOneFakePackage("B");
			
			SetFilterParameter("B");
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				package
			};
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedWithFilter_ReturnsFilteredUpdatedPackages()
		{
			CreateCmdlet();
			AddDefaultProjectToConsoleHost();
			AddPackageToProjectManagerLocalRepository("A", "1.0.0.0");
			AddPackageToAggregateRepository("A", "1.1.0.0");
			AddPackageToProjectManagerLocalRepository("B", "2.0.0.0");
			var updatedPackage = AddPackageToAggregateRepository("B", "2.1.0.0");
			
			EnableUpdatesParameter();
			SetFilterParameter("B");
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenPackageSourceParameterSpecified_PackageRepositoryCreatedForPackageSourceSpecifiedByParameter()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			
			SetSourceParameter("http://sharpdevelop.com/packages");
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToCreatePackageRepository.Source;
			var expectedPackageSource = "http://sharpdevelop.com/packages";
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_JustSourceParameterPassed_PackageRepositoryCreatedForPackageSourceSpecifiedByParameter()
		{
			CreateCmdlet();
			SetSourceParameter("http://test");
			
			RunCmdlet();
			
			var actualPackageSource = fakePackageManagementService.PackageSourcePassedToCreatePackageRepository.Source;
			var expectedPackageSource = "http://test";
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_RecentPackagesRequested_RecentPackagesReturned()
		{
			CreateCmdlet();
			
			var recentPackageRepository = fakePackageManagementService.FakeRecentPackageRepository;
			recentPackageRepository.AddOneFakePackage("A");
			
			EnableRecentParameter();
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = recentPackageRepository.FakePackages;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_RecentPackagesRequestedWithFilter_FilteredRecentPackagesReturned()
		{
			CreateCmdlet();
			
			var recentPackageRepository = fakePackageManagementService.FakeRecentPackageRepository;
			recentPackageRepository.AddOneFakePackage("A");
			var packageB = recentPackageRepository.AddOneFakePackage("B");
			
			EnableRecentParameter();
			SetFilterParameter("B");
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				packageB
			};
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_FilterParameterPassedContainingTwoSearchTermsSeparatedBySpaceCharacter_InstallPackagesAreFilteredByBothSearchTerms()
		{
			CreateCmdlet();
			AddPackageSourceToConsoleHost();
			FakeProjectManager projectManager = fakePackageManagementService.FakeProjectManagerToReturnFromCreateProjectManager;
			var packageA = projectManager.FakeLocalRepository.AddOneFakePackage("A");
			var packageB = projectManager.FakeLocalRepository.AddOneFakePackage("B");
			var packageC = projectManager.FakeLocalRepository.AddOneFakePackage("C");
			
			SetFilterParameter("B C");
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				packageB,
				packageC
			};
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_RetrieveUpdatesWhenNoProjectIsOpen_ThrowsTerminatingError()
		{
			CreateCmdlet();
			EnableUpdatesParameter();
			
			RunCmdlet();
			
			Assert.IsTrue(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_RetrieveUpdatesWhenProjectIsActive_DoesNotThrowTerminatingError()
		{
			CreateCmdlet();
			AddDefaultProjectToConsoleHost();
			EnableUpdatesParameter();
			
			RunCmdlet();
			
			Assert.IsFalse(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_NoParametersSetAndNoProjectIsOpen_ThrowsNoProjectOpenTerminatingError()
		{
			CreateCmdlet();
			RunCmdlet();
			
			Assert.IsTrue(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesAndProjectIsNotOpen_NoTerminatingErrorIsThrown()
		{
			CreateCmdlet();
			EnableListAvailableParameter();
			RunCmdlet();
			
			Assert.IsFalse(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_RecentPackagesRequestedAndProjectIsNotOpen_NoTerminatingErrorIsThrown()
		{
			CreateCmdlet();
			EnableRecentParameter();
			RunCmdlet();
			
			Assert.IsFalse(fakeTerminatingError.IsThrowNoProjectOpenErrorCalled);
		}
		
		[Test]
		public void ProcessRecord_ListAvailableAndSkipFirstTwoPackages_ReturnsAllPackagesExceptionFirstTwo()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			repository.AddOneFakePackage("A");
			repository.AddOneFakePackage("B");
			var packageC = repository.AddOneFakePackage("C");			
			
			EnableListAvailableParameter();
			SetSkipParameter(2);
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				packageC
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void Skip_NewGetPackageCmdletInstance_ReturnsZero()
		{
			CreateCmdlet();
			int skip = cmdlet.Skip;
			
			Assert.AreEqual(0, skip);
		}
		
		[Test]
		public void ProcessRecord_ListAvailableAndTakeTwo_ReturnsFirstTwoPackages()
		{
			CreateCmdlet();
			var repository = fakePackageManagementService.FakePackageRepositoryToReturnFromCreatePackageRepository;
			var packageA = repository.AddOneFakePackage("A");
			var packageB = repository.AddOneFakePackage("B");
			repository.AddOneFakePackage("C");			
			
			EnableListAvailableParameter();
			SetTakeParameter(2);
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				packageA,
				packageB
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void Take_NewGetPackageCmdletInstance_ReturnsZero()
		{
			CreateCmdlet();
			int take = cmdlet.Take;
			
			Assert.AreEqual(0, take);
		}
	}
}
