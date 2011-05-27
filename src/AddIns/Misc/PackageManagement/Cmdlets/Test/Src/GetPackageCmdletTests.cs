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
	public class GetPackageCmdletTests : CmdletTestsBase
	{
		TestableGetPackageCmdlet cmdlet;
		FakePackageManagementSolution fakeSolution;
		FakeRegisteredPackageRepositories fakeRegisteredPackageRepositories;
		FakeCommandRuntime fakeCommandRuntime;
		FakeCmdletTerminatingError fakeTerminatingError;
		
		void CreateCmdlet()
		{
			cmdlet = new TestableGetPackageCmdlet();
			fakeSolution = new FakePackageManagementSolution();
			fakeRegisteredPackageRepositories = cmdlet.FakeRegisteredPackageRepositories;
			fakeConsoleHost = cmdlet.FakePackageManagementConsoleHost;
			fakeConsoleHost.FakeProject = fakeSolution.FakeProject;
			fakeCommandRuntime = cmdlet.FakeCommandRuntime;
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			
			AddDefaultProjectToConsoleHost();
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
			fakeSolution.AddPackageToActiveProjectLocalRepository(package);
			return package;
		}
		
		FakePackage AddPackageToAggregateRepository(string version)
		{
			return AddPackageToAggregateRepository("Test", version);
		}
		
		FakePackage AddPackageToAggregateRepository(string id, string version)
		{
			return fakeRegisteredPackageRepositories.AddFakePackageWithVersionToAggregrateRepository(id, version);
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
		
		void SetFirstParameter(int first)
		{
			cmdlet.First = first;
		}

		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasOnePackage_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("Test");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = repository.FakePackages;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasOnePackageAndNoDefaultProjectIsSet_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			fakeConsoleHost.DefaultProject = null;
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("Test");
			
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
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("A");
			repository.AddFakePackage("B");
			repository.AddFakePackage("C");
			
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
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			var packageB = repository.AddFakePackage("B");
			var packageA = repository.AddFakePackage("A");
			
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
			fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource = source;
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(source, actualSource);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_ReturnsPackagesInstalledForProjectSelectedInConsole()
		{
			CreateCmdlet();
			fakeSolution.AddPackageToActiveProjectLocalRepository("One");
			fakeSolution.AddPackageToActiveProjectLocalRepository("Two");
			
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = fakeConsoleHost.FakeProject.FakePackages;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_NullPackageSourceUsedWhenCreatingProject()
		{
			CreateCmdlet();
			
			RunCmdlet();
			
			var actualSource = fakeConsoleHost.PackageSourcePassedToGetProject;
			
			Assert.IsNull(actualSource);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_DefaultProjectInConsoleHostUsedToCreateProject()
		{
			CreateCmdlet();
			var project = AddDefaultProjectToConsoleHost();
			project.Name = "MyProject";
			
			RunCmdlet();
			
			var actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequested_ReturnsUpdatedPackagesForActiveProject()
		{
			CreateCmdlet();
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
		public void ProcessRecord_UpdatedPackagesRequested_ActiveProjectNameUsedWhenCreatingProject()
		{
			CreateCmdlet();
			var project = AddDefaultProjectToConsoleHost();
			project.Name = "Test";
			EnableUpdatesParameter();
			RunCmdlet();
			
			var actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("Test", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequested_AggregateRepositoryUsedWhenCreatingProject()
		{
			CreateCmdlet();
			EnableUpdatesParameter();
			RunCmdlet();
			
			var actualRepository = fakeConsoleHost.PackageRepositoryPassedToGetProject;
			var expectedRepository = fakeRegisteredPackageRepositories.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesAndFilterResults_PackagesReturnedMatchFilter()
		{
			CreateCmdlet();
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("A");
			var package = repository.AddFakePackage("B");
			repository.AddFakePackage("C");
			
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
		public void ProcessRecord_FilterParameterPassed_InstalledPackagesAreFiltered()
		{
			CreateCmdlet();
			AddPackageSourceToConsoleHost();
			
			fakeSolution.AddPackageToActiveProjectLocalRepository("A");
			var package = fakeSolution.AddPackageToActiveProjectLocalRepository("B");
			
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
			var expectedPackageSource = new PackageSource("http://sharpdevelop.com/packages");
			fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource = expectedPackageSource;
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			
			SetSourceParameter("http://sharpdevelop.com/packages");
			EnableListAvailableParameter();
			RunCmdlet();
			
			var actualPackageSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_JustSourceParameterPassed_ProjectCreatedForPackageSourceSpecifiedByParameter()
		{
			CreateCmdlet();
			
			SetSourceParameter("http://test");
			
			RunCmdlet();
			
			var actualPackageSource = fakeConsoleHost.PackageSourcePassedToGetProject;
			var expectedPackageSource = "http://test";
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_JustSourceParameterPassed_ProjectCreatedForPackageSource()
		{
			CreateCmdlet();
			fakeConsoleHost.DefaultProject.Name = "MyProject";
			SetSourceParameter("http://test");
			
			RunCmdlet();
			
			var actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_RecentPackagesRequested_RecentPackagesReturned()
		{
			CreateCmdlet();
			
			var recentPackageRepository = fakeRegisteredPackageRepositories.FakeRecentPackageRepository;
			recentPackageRepository.AddFakePackage("A");
			
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
			
			var recentPackageRepository = fakeRegisteredPackageRepositories.FakeRecentPackageRepository;
			recentPackageRepository.AddFakePackage("A");
			var packageB = recentPackageRepository.AddFakePackage("B");
			
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
			var packageA = fakeSolution.AddPackageToActiveProjectLocalRepository("A");
			var packageB = fakeSolution.AddPackageToActiveProjectLocalRepository("B");
			var packageC = fakeSolution.AddPackageToActiveProjectLocalRepository("C");
			
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
			fakeConsoleHost.DefaultProject = null;
			EnableUpdatesParameter();
			
			Assert.Throws(typeof(FakeCmdletTerminatingErrorException), () => RunCmdlet());
		}
		
		[Test]
		public void ProcessRecord_NoParametersSetAndNoProjectIsOpen_ThrowsNoProjectOpenTerminatingError()
		{
			CreateCmdlet();
			fakeConsoleHost.DefaultProject = null;
			
			Assert.Throws(typeof(FakeCmdletTerminatingErrorException), () => RunCmdlet());
		}
		
		[Test]
		public void ProcessRecord_ListAvailableAndSkipFirstTwoPackages_ReturnsAllPackagesExceptionFirstTwo()
		{
			CreateCmdlet();
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("A");
			repository.AddFakePackage("B");
			var packageC = repository.AddFakePackage("C");			
			
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
		public void ProcessRecord_ListAvailableAndFirstTwo_ReturnsFirstTwoPackages()
		{
			CreateCmdlet();
			var repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			var packageA = repository.AddFakePackage("A");
			var packageB = repository.AddFakePackage("B");
			repository.AddFakePackage("C");			
			
			EnableListAvailableParameter();
			SetFirstParameter(2);
			RunCmdlet();
			
			var actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				packageA,
				packageB
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void First_NewGetPackageCmdletInstance_ReturnsZero()
		{
			CreateCmdlet();
			int first = cmdlet.First;
			
			Assert.AreEqual(0, first);
		}
	}
}
