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
		
		void EnablePrereleaseParameter()
		{
			cmdlet.IncludePrerelease = new SwitchParameter(true);
		}
		
		void EnableAllVersionsParameter()
		{
			cmdlet.AllVersions = new SwitchParameter(true);
		}
		
		FakePackage AddPackageToSpecifiedProjectManagerLocalRepository(string id, string version)
		{
			FakePackage package = FakePackage.CreatePackageWithVersion(id, version);
			fakeConsoleHost.FakeProject.FakePackages.Add(package);
			return package;
		}
		
		FakePackage AddPackageToSelectedRepositoryInConsoleHost(string id)
		{
			return AddPackageToSelectedRepositoryInConsoleHost(id, "1.0");
		}
		
		FakePackage AddPackageToSelectedRepositoryInConsoleHost(string id, string version)
		{
			return fakeRegisteredPackageRepositories.FakePackageRepository.AddFakePackageWithVersion(id, version);
		}
		
		void SetFilterParameter(string filter)
		{
			cmdlet.Filter = filter;
		}
		
		void SetSourceParameter(string source)
		{
			cmdlet.Source = source;
		}
		
		void SetSkipParameter(int skip)
		{
			cmdlet.Skip = skip;
		}
		
		void SetFirstParameter(int first)
		{
			cmdlet.First = first;
		}
		
		void AddPackageToSolution(string id, string version)
		{
			fakeSolution.AddPackageToSharedLocalRepository(id, version);
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
			AddPackageToSolution("Test", "1.0.0.0");
			FakePackage updatedPackage = AddPackageToSelectedRepositoryInConsoleHost("Test", "1.1.0.0");
			EnableUpdatesParameter();
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedAndProjectNameSpecified_ActiveRepositoryUsedWhenCreatingProject()
		{
			CreateCmdlet();
			EnableUpdatesParameter();
			cmdlet.ProjectName = "MyProject";
			RunCmdlet();
			
			IPackageRepository actualRepository = fakeConsoleHost.PackageRepositoryPassedToGetProject;
			FakePackageRepository expectedRepository = fakeRegisteredPackageRepositories.FakePackageRepository;
			Assert.AreEqual(expectedRepository, actualRepository);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesRequestedWhenNoPackageSourceSpecified_PackageSourceTakenFromConsoleHost()
		{
			CreateCmdlet();
			PackageSource source = AddPackageSourceToConsoleHost();
			fakeConsoleHost.PackageSourceToReturnFromGetActivePackageSource = source;
			EnableUpdatesParameter();
			
			RunCmdlet();
			
			PackageSource actualSource = fakeRegisteredPackageRepositories.PackageSourcePassedToCreateRepository;
			Assert.AreEqual(source, actualSource);
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
			AddPackageToSelectedRepositoryInConsoleHost("A", "1.1.0.0");
			fakeSolution.AddPackageToSharedLocalRepository("B", "2.0.0.0");
			FakePackage updatedPackage = AddPackageToSelectedRepositoryInConsoleHost("B", "2.1.0.0");
			
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
			AddPackageToSelectedRepositoryInConsoleHost("A", "1.1.0.0");
			AddPackageToSpecifiedProjectManagerLocalRepository("B", "2.0.0.0");
			FakePackage updatedPackage = AddPackageToSelectedRepositoryInConsoleHost("B", "2.1.0.0");
			
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
		
		[Test]
		public void ProcessRecord_ListAvailableAndPrereleasePackagesWithFilter_ReturnsFilteredPrereleasePackages()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			FakePackage expectedPackage = repository.AddFakePackageWithVersion("B", "2.1.0-beta");
			EnableListAvailableParameter();
			EnablePrereleaseParameter();
			SetFilterParameter("B");
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				expectedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailableWithFilterAndOnlineRepositoryHasPrereleasePackages_ReturnsNoPrereleasePackages()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			FakePackage expectedPackage = repository.AddFakePackageWithVersion("B", "2.1.0");
			FakePackage package = repository.AddFakePackageWithVersion("B", "2.1.0-beta");
			EnableListAvailableParameter();
			SetFilterParameter("B");
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				expectedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailableAndPrereleasePackagesWithFilterWhenMultiplePackageVersions_ReturnsLatestPrereleasePackages()
		{
			CreateCmdlet();
			FakePackageRepository repository = fakeRegisteredPackageRepositories.FakePackageRepository;
			repository.AddFakePackageWithVersion("B", "2.0.1-beta");
			FakePackage expectedPackage = repository.AddFakePackageWithVersion("B", "2.1.0-beta");
			EnableListAvailableParameter();
			EnablePrereleaseParameter();
			SetFilterParameter("B");
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				expectedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_NoParametersPassedWhenMultipleVersionsOfPackageInstalledInSolution_ReturnsAllPackageVersions()
		{
			CreateCmdlet();
			FakePackage expectedPackage1 = fakeSolution.AddPackageToSharedLocalRepository("One", "1.0");
			FakePackage expectedPackage2 = fakeSolution.AddPackageToSharedLocalRepository("One", "1.1");
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				expectedPackage1,
				expectedPackage2
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesIncludingPrereleasesRequestedAndNoProjectName_ReturnsUpdatedPrereleasePackagesForSolution()
		{
			CreateCmdlet();
			AddPackageToSolution("Test", "1.0.0.0");
			FakePackage updatedPackage = AddPackageToSelectedRepositoryInConsoleHost("Test", "1.1.0-alpha");
			EnableUpdatesParameter();
			EnablePrereleaseParameter();
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_UpdatedPackagesOnlyRequestedAndNoProjectNameAndRepositoryHasPrereleases_ReturnsUpdatedPackagesButNoPrereleasesForSolution()
		{
			CreateCmdlet();
			AddPackageToSolution("Test", "1.0.0.0");
			AddPackageToSelectedRepositoryInConsoleHost("Test", "1.2.0-alpha");
			FakePackage updatedPackage = AddPackageToSelectedRepositoryInConsoleHost("Test", "1.1.0");
			EnableUpdatesParameter();
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			var expectedPackages = new FakePackage[] {
				updatedPackage
			};
			
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesAndTwoVersionsOfSamePackageInOnlineRepository_OutputIsLatestPackageVersion()
		{
			CreateCmdlet();
			AddPackageToSelectedRepositoryInConsoleHost("Test", "1.0");
			FakePackage expectedPackage = AddPackageToSelectedRepositoryInConsoleHost("Test", "1.1");
			EnableListAvailableParameter();
			var expectedPackages = new FakePackage[] {
				expectedPackage,
			};
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void ProcessRecord_ListAvailablePackagesAndAllVersionsAndTwoVersionsOfSamePackageInOnlineRepository_OutputIsAllPackageVersions()
		{
			CreateCmdlet();
			FakePackage expectedPackage1 = AddPackageToSelectedRepositoryInConsoleHost("Test", "1.0");
			expectedPackage1.IsLatestVersion = false;
			FakePackage expectedPackage2 = AddPackageToSelectedRepositoryInConsoleHost("Test", "1.1");
			EnableListAvailableParameter();
			EnableAllVersionsParameter();
			var expectedPackages = new FakePackage[] {
				expectedPackage1,
				expectedPackage2
			};
			
			RunCmdlet();
			
			List<object> actualPackages = fakeCommandRuntime.ObjectsPassedToWriteObject;
			CollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
	}
}
