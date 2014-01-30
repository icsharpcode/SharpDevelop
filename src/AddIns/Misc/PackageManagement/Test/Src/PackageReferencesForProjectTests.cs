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
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageReferencesForProjectTests
	{
		PackageReferencesForProject packageReferencesForProject;
		FakePackageReferenceFileFactory fakePackageReferenceFileFactory;
		FakePackageReferenceFile fakePackageReferenceFile;
		FakePackageReferenceInstaller fakePackageReferenceInstaller;
		
		TestableProject CreateProject()
		{
			return ProjectHelper.CreateTestProject();
		}
		
		void CreatePackageReferencesForProject(MSBuildBasedProject project)
		{
			fakePackageReferenceFileFactory = new FakePackageReferenceFileFactory();
			fakePackageReferenceFile = fakePackageReferenceFileFactory.FakePackageReferenceFile;
			fakePackageReferenceInstaller = new FakePackageReferenceInstaller();
			packageReferencesForProject = 
				new PackageReferencesForProject(
					project,
					fakePackageReferenceInstaller,
					fakePackageReferenceFileFactory);
		}
		
		void AddPackageReference(string packageId, string version)
		{
			fakePackageReferenceFile.AddFakePackageReference(packageId, version);
		}
		
		void PackageReferenceCollectionAssertAreEqual(IEnumerable<PackageReference> expected, IEnumerable<PackageReference> actual)
		{
			IEnumerable<string> expectedAsStrings = ConvertToStrings(expected);
			IEnumerable<string> actualAsStrings = ConvertToStrings(actual);
			
			CollectionAssert.AreEqual(expectedAsStrings, actualAsStrings);
		}
		
		List<string> ConvertToStrings(IEnumerable<PackageReference> packageReferences)
		{
			var converted = new List<string>();
			foreach (PackageReference packageReference in packageReferences) {
				string convertedPackageReference = ConvertToString(packageReference);
				converted.Add(convertedPackageReference);
			}
			return converted;
		}
		
		string ConvertToString(PackageReference packageReference)
		{
			return String.Format(
				"Id: {0}, Version: {1}",
				packageReference.Id,
				packageReference.Version);
		}
		
		[Test]
		public void InstallPackages_OnePackageReferenceInPackageConfigFile_OpensPackageConfigFileInProjectDirectory()
		{
			TestableProject project = CreateProject();
			project.FileName = new FileName(@"d:\projects\myproject\myproject.csproj");
			CreatePackageReferencesForProject(project);
			AddPackageReference("PackageId", "1.0.3.2");
			
			packageReferencesForProject.InstallPackages();
			
			string fileName = fakePackageReferenceFileFactory.FileNamePassedToCreatePackageReferenceFile;
			string expectedFileName = @"d:\projects\myproject\packages.config";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void InstallPackages_OnePackageReferenceInPackageConfigFile_OnePackageInstalled()
		{
			TestableProject project = CreateProject();
			CreatePackageReferencesForProject(project);
			AddPackageReference("PackageId", "1.0.3.2");
			
			packageReferencesForProject.InstallPackages();
			
			IEnumerable<PackageReference> packageReferencesInstalled = 
				fakePackageReferenceInstaller.PackageReferencesPassedToInstallPackages;
			
			List<PackageReference> expectedPackageReferences = 
				fakePackageReferenceFile.FakePackageReferences;
			
			CollectionAssert.AreEqual(expectedPackageReferences, packageReferencesInstalled);
		}
		
		[Test]
		public void InstallPackages_OnePackageReferenceInPackageConfigFile_ProjectPassedToInstaller()
		{
			TestableProject expectedProject = CreateProject();
			CreatePackageReferencesForProject(expectedProject);
			AddPackageReference("PackageId", "1.0.3.2");
			
			packageReferencesForProject.InstallPackages();
			
			MSBuildBasedProject project = fakePackageReferenceInstaller.ProjectPassedToInstallPackages;
			
			Assert.AreEqual(expectedProject, project);
		}
		
		[Test]
		public void RemovePackageReferences_TwoPackageReferencesInPackageConfigFile_PackageReferenceFileIsDeleted()
		{			
			TestableProject project = CreateProject();
			CreatePackageReferencesForProject(project);
			AddPackageReference("One", "1.0.3.2");
			AddPackageReference("Two", "2.0.44");
			
			packageReferencesForProject.RemovePackageReferences();
			
			bool deleted = fakePackageReferenceFile.IsDeleteCalled;
			
			Assert.IsTrue(deleted);
		}
	}
}
