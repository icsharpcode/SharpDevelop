// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
			fakeConsoleHost.FakeSolution = fakeSolution;
			fakeConsoleHost.FakeProject = new FakePackageManagementProject();
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
		
		FakePackage AddPackageToSpecifiedProjectManagerLocalRepository(string id, string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(id, version);
			fakeConsoleHost.FakeProject.FakePackages.Add(package);
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
		
		void AddPackageWithVersionToSolution(string version)
		{
			fakeSolution.AddPackageToSharedLocalRepository("Test", version);
		}

		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasOnePackage_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("Test");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			List<FakePackage> expectedPackages = repository.FakePackages;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasOnePackageAndNoDefaultProjectIsSet_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			fakeConsoleHost.DefaultProject = null;
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("Test");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			List<FakePackage> expectedPackages = repository.FakePackages;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasThreePackages_OutputIsPackagesFromPackageSourceRepository()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("A");
			repository.AddFakePackage("B");
			repository.AddFakePackage("C");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			List<FakePackage> expectedPackages = repository.FakePackages;
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenDefaultPackageSourceHasTwoPackages_PackagesAreSortedById()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			FakePackage packageB = repository.AddFakePackage("B");
			FakePackage packageA = repository.AddFakePackage("A");
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
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
			PackageSource source = AddPackageSourceToConsoleHost();
			fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource = source;
			
			EnableListAvailableParameter();
			RunCmdlet();
			
			PackageSource actualSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(source, actualSource);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassed_ReturnsPackagesInstalledForSolution()
		{
			CreateCmdlet();
			fakeSolution.AddPackageToSharedLocalRepository("One");
			fakeSolution.AddPackageToSharedLocalRepository("Two");
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			List<FakePackage> expectedPackages = fakeSolution.FakeInstalledPackages;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_NullPackageSourceUsedWhenCreatingProject()
		{
			CreateCmdlet();
			cmdlet.ProjectName = "Test";
			RunCmdlet();
			
			string actualSource = fakeConsoleHost.PackageSourcePassedToGetProject;
			
			Assert.IsNull(actualSource);
		}
		
		[Test]
		public void ProcessRecord_ProjectNameSpecified_ProjectNameUsedToCreateProject()
		{
			CreateCmdlet();
			cmdlet.ProjectName = "MyProject";
			
			RunCmdlet();
			
			string actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedAndNoProjectName_ReturnsUpdatedPackagesForSolution()
		{
			CreateCmdlet();
			AddPackageWithVersionToSolution("1.0.0.0");
			FakePackage updatedPackage = AddPackageToAggregateRepository("1.1.0.0");
			
			EnableUpdatesParameter();
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedAndProjectNameSpecified_AggregateRepositoryUsedWhenCreatingProject()
		{
			CreateCmdlet();
			EnableUpdatesParameter();
			cmdlet.ProjectName = "MyProject";
			RunCmdlet();
			
			IPackageRepository actualRepository = fakeConsoleHost.PackageRepositoryPassedToGetProject;
			FakePackageRepository expectedRepository = fakeRegisteredPackageRepositories.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesAndFilterResults_PackagesReturnedMatchFilter()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("A");
			FakePackage package = repository.AddFakePackage("B");
			repository.AddFakePackage("C");
			
			EnableListAvailableParameter();
			SetFilterParameter("B");
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				 package
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_FilterParameterPassed_InstalledPackagesInSolutionAreFiltered()
		{
			CreateCmdlet();
			AddPackageSourceToConsoleHost();
			
			fakeSolution.AddPackageToSharedLocalRepository("A");
			FakePackage package = fakeSolution.AddPackageToSharedLocalRepository("B");
			
			SetFilterParameter("B");
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				package
			};
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedWithFilter_ReturnsFilteredUpdatedPackagesFromSolution()
		{
			CreateCmdlet();
			fakeSolution.AddPackageToSharedLocalRepository("A", "1.0.0.0");
			AddPackageToAggregateRepository("A", "1.1.0.0");
			fakeSolution.AddPackageToSharedLocalRepository("B", "2.0.0.0");
			FakePackage updatedPackage = AddPackageToAggregateRepository("B", "2.1.0.0");
			
			EnableUpdatesParameter();
			SetFilterParameter("B");
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedWithFilterAndProjectName_ReturnsFilteredUpdatedPackagesFromProject()
		{
			CreateCmdlet();
			AddPackageToSpecifiedProjectManagerLocalRepository("A", "1.0.0.0");
			AddPackageToAggregateRepository("A", "1.1.0.0");
			AddPackageToSpecifiedProjectManagerLocalRepository("B", "2.0.0.0");
			FakePackage updatedPackage = AddPackageToAggregateRepository("B", "2.1.0.0");
			
			EnableUpdatesParameter();
			SetFilterParameter("B");
			cmdlet.ProjectName = "MyProject";
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
			Assert.AreEqual("MyProject", fakeConsoleHost.ProjectNamePassedToGetProject);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesWhenPackageSourceParameterSpecified_PackageRepositoryCreatedForPackageSourceSpecifiedByParameter()
		{
			CreateCmdlet();
			var expectedPackageSource = new PackageSource("http://sharpdevelop.com/packages");
			fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource = expectedPackageSource;
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			
			SetSourceParameter("http://sharpdevelop.com/packages");
			EnableListAvailableParameter();
			RunCmdlet();
			
			PackageSource actualPackageSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_SourceParameterAndProjectNamePassed_ProjectCreatedForPackageSourceSpecifiedByParameter()
		{
			CreateCmdlet();
			
			SetSourceParameter("http://test");
			cmdlet.ProjectName = "MyProject";
			RunCmdlet();
			
			string actualPackageSource = fakeConsoleHost.PackageSourcePassedToGetProject;
			string expectedPackageSource = "http://test";
			
			Assert.AreEqual(expectedPackageSource, actualPackageSource);
		}
		
		[Test]
		public void ProcessRecord_SourceParameterAndProjectNamePassed_ProjectCreatedForPackageSource()
		{
			CreateCmdlet();
			cmdlet.ProjectName = "MyProject";
			SetSourceParameter("http://test");
			
			RunCmdlet();
			
			string actualProjectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("MyProject", actualProjectName);
		}
		
		[Test]
		public void ProcessRecord_RecentPackagesRequested_RecentPackagesReturned()
		{
			CreateCmdlet();
			
			FakePackageRepository recentPackageRepository = fakeRegisteredPackageRepositories.FakeRecentPackageRepository;
			recentPackageRepository.AddFakePackage("A");
			
			EnableRecentParameter();
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			List<FakePackage> expectedPackages = recentPackageRepository.FakePackages;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_RecentPackagesRequestedWithFilter_FilteredRecentPackagesReturned()
		{
			CreateCmdlet();
			
			FakePackageRepository recentPackageRepository = fakeRegisteredPackageRepositories.FakeRecentPackageRepository;
			recentPackageRepository.AddFakePackage("A");
			FakePackage packageB = recentPackageRepository.AddFakePackage("B");
			
			EnableRecentParameter();
			SetFilterParameter("B");
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
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
			FakePackage packageA = fakeSolution.AddPackageToSharedLocalRepository("A");
			FakePackage packageB = fakeSolution.AddPackageToSharedLocalRepository("B");
			FakePackage packageC = fakeSolution.AddPackageToSharedLocalRepository("C");
			
			SetFilterParameter("B C");
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
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
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackage("A");
			repository.AddFakePackage("B");
			FakePackage packageC = repository.AddFakePackage("C");			
			
			EnableListAvailableParameter();
			SetSkipParameter(2);
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
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
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			FakePackage packageA = repository.AddFakePackage("A");
			FakePackage packageB = repository.AddFakePackage("B");
			repository.AddFakePackage("C");			
			
			EnableListAvailableParameter();
			SetFirstParameter(2);
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
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
		
		[Test]
		public void ProcessRecord_GetInstalledPackagesWhenProjectNameSpecified_ProjectNameParameterUsedToGetProject()
		{
			CreateCmdlet();
			cmdlet.ProjectName = "Test";
			RunCmdlet();
			
			string projectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("Test", projectName);
		}
		
		[Test]
		public void ProcessRecord_GetInstalledPackagesWhenProjectNameSpecified_ReturnsPackagesInstalledForProject()
		{
			CreateCmdlet();
			var project = new FakePackageManagementProject();
			fakeConsoleHost.FakeProject = project;
			project.AddFakePackage("One");
			project.AddFakePackage("Two");
			cmdlet.ProjectName = "Test";
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			List<FakePackage> expectedPackages = project.FakePackages;
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_GetUpdatedPackagesWhenProjectNameSpecified_ProjectNameParameterUsedToGetProject()
		{
			CreateCmdlet();
			cmdlet.ProjectName = "Test";
			EnableUpdatesParameter();
			RunCmdlet();
			
			string projectName = fakeConsoleHost.ProjectNamePassedToGetProject;
			
			Assert.AreEqual("Test", projectName);
		}
	}
}
