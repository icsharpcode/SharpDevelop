// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageReferenceInstallerTests
	{
		PackageReferenceInstaller installer;
		FakePackageActionRunner fakeActionRunner;
		FakePackageManagementProjectFactory fakeProjectFactory;
		TestableProject project;
		List<PackageReference> packageReferences;
		FakePackageRepositoryFactory fakeRepositoryCache;
		
		void CreateInstaller()
		{
			project = ProjectHelper.CreateTestProject();
			packageReferences = new List<PackageReference>();
			fakeActionRunner = new FakePackageActionRunner();
			fakeProjectFactory = new FakePackageManagementProjectFactory();
			fakeRepositoryCache = new FakePackageRepositoryFactory();
			installer = new PackageReferenceInstaller(fakeRepositoryCache, fakeActionRunner, fakeProjectFactory);
		}
		
		void InstallPackages()
		{
			installer.InstallPackages(packageReferences, project);
		}
		
		void AddPackageReference(string packageId, string version)
		{
			var packageReference = new PackageReference(packageId, new SemanticVersion(version), null, null);
			packageReferences.Add(packageReference);
		}
		
		[Test]
		public void InstallPackages_OnePackageReference_OnePackageIsInstalled()
		{
			CreateInstaller();
			AddPackageReference("PackageId", "1.3.4.5");
			InstallPackages();
			
			var actions = new List<ProcessPackageAction>(fakeActionRunner.ActionsRunInOneCall);
			var action = actions[0] as InstallPackageAction;
			
			var expectedVersion = new SemanticVersion("1.3.4.5");
			
			Assert.AreEqual("PackageId", action.PackageId);
			Assert.AreEqual(expectedVersion, action.PackageVersion);
		}
		
		[Test]
		public void InstallPackages_OnePackageReference_PackageManagementProjectIsCreatedForMSBuildProject()
		{
			CreateInstaller();
			AddPackageReference("PackageId", "1.3.4.5");
			InstallPackages();
			
			MSBuildBasedProject projectUsedToCreatePackageManagementProject
				= fakeProjectFactory.FirstProjectPassedToCreateProject;
			
			Assert.AreEqual(project, projectUsedToCreatePackageManagementProject);
		}
		
		[Test]
		public void InstallPackages_OnePackageReference_AggregateRepositoryFromCacheUsedToCreatePackageManagementProject()
		{
			CreateInstaller();
			AddPackageReference("PackageId", "1.3.4.5");
			InstallPackages();
			
			IPackageRepository repository = fakeProjectFactory.FirstRepositoryPassedToCreateProject;
			IPackageRepository expectedRepository = fakeRepositoryCache.FakeAggregateRepository;
			
			Assert.AreEqual(expectedRepository, repository);		
		}
	}
}
