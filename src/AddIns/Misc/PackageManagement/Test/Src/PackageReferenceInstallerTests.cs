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
			var packageReference = new PackageReference(packageId, new SemanticVersion(version), null, null, false, false);
			packageReferences.Add(packageReference);
		}
		
		[Test]
		public void InstallPackages_OnePackageReference_OnePackageIsInstalled()
		{
			CreateInstaller();
			AddPackageReference("PackageId", "1.3.4.5");
			InstallPackages();
			
			var actions = new List<IPackageAction>(fakeActionRunner.ActionsRunInOneCall);
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
