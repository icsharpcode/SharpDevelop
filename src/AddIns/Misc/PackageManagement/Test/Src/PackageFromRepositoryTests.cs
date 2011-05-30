// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageFromRepositoryTests
	{
		FakePackage fakePackage;
		PackageFromRepository package;
		FakePackageRepository fakeRepository;
		
		void CreatePackage()
		{
			fakePackage = new FakePackage("Test");
			fakeRepository = new FakePackageRepository();
			package = new PackageFromRepository(fakePackage, fakeRepository);
		}
		
		[Test]
		public void Repository_PackageCreatedWithSourceRepository_ReturnsSourceRepository()
		{
			CreatePackage();
			var repository = package.Repository;
			
			Assert.AreEqual(fakeRepository, repository);
		}
		
		[Test]
		public void AssemblyReferences_WrappedPackageHasOneAssemblyReference_ReturnsOneAssemblyReference()
		{
			CreatePackage();
			fakePackage.AssemblyReferenceList.Add(new FakePackageAssemblyReference());
			
			var assemblyReferences = package.AssemblyReferences;
			var expectedAssemblyReferences = fakePackage.AssemblyReferenceList;
			
			CollectionAssert.AreEqual(expectedAssemblyReferences, assemblyReferences);
		}
		
		[Test]
		public void Id_WrappedPackageIdIsTest_ReturnsTest()
		{
			CreatePackage();
			fakePackage.Id = "Test";
			string id = package.Id;
			
			Assert.AreEqual("Test", id);
		}
		
		[Test]
		public void Version_WrappedPackageVersionIsOnePointOne_ReturnsOnePointOne()
		{
			CreatePackage();
			var expectedVersion = new Version("1.1");
			fakePackage.Version = expectedVersion;
			var version = package.Version;
			
			Assert.AreEqual(expectedVersion, version);
		}
		
		[Test]
		public void Title_WrappedPackageTitleIsTest_ReturnsTest()
		{
			CreatePackage();
			fakePackage.Title = "Test";
			var title = package.Title;
			
			Assert.AreEqual("Test", title);
		}
		
		[Test]
		public void Authors_WrappedPackageHasOneAuthor_ReturnsOneAuthor()
		{
			CreatePackage();
			fakePackage.AuthorsList.Add("Author1");
			
			var authors = package.Authors;
			var expectedAuthors = fakePackage.AuthorsList;
			
			CollectionAssert.AreEqual(expectedAuthors, authors);
		}
		
		[Test]
		public void Owners_WrappedPackageHasOneOwner_ReturnsOneOwner()
		{
			CreatePackage();
			fakePackage.OwnersList.Add("Owner1");
			
			var owners = package.Owners;
			var expectedOwners = fakePackage.OwnersList;
			
			CollectionAssert.AreEqual(expectedOwners, owners);
		}
		
		[Test]
		public void IconUrl_WrappedPackageIconUrlIsHttpSharpDevelopNet_ReturnsHttpSharpDevelopNet()
		{
			CreatePackage();
			var expectedUrl = new Uri("http://sharpdevelop.net");
			fakePackage.IconUrl = expectedUrl;
			var url = package.IconUrl;
			
			Assert.AreEqual(expectedUrl, url);
		}
		
		[Test]
		public void LicenseUrl_WrappedPackageLicenseUrlIsHttpSharpDevelopNet_ReturnsHttpSharpDevelopNet()
		{
			CreatePackage();
			var expectedUrl = new Uri("http://sharpdevelop.net");
			fakePackage.LicenseUrl = expectedUrl;
			var url = package.LicenseUrl;
			
			Assert.AreEqual(expectedUrl, url);
		}
		
		[Test]
		public void ProjectUrl_WrappedPackageProjectUrlIsHttpSharpDevelopNet_ReturnsHttpSharpDevelopNet()
		{
			CreatePackage();
			var expectedUrl = new Uri("http://sharpdevelop.net");
			fakePackage.ProjectUrl = expectedUrl;
			var url = package.ProjectUrl;
			
			Assert.AreEqual(expectedUrl, url);
		}
		
		[Test]
		public void ReportAbuseUrl_WrappedPackageReportAbuseUrlIsHttpSharpDevelopNet_ReturnsHttpSharpDevelopNet()
		{
			CreatePackage();
			var expectedUrl = new Uri("http://sharpdevelop.net");
			fakePackage.ReportAbuseUrl = expectedUrl;
			var url = package.ReportAbuseUrl;
			
			Assert.AreEqual(expectedUrl, url);
		}
		
		[Test]
		public void RequiresLicenseAcceptance_WrappedPackageRequiresLicenseAcceptanceIsTrue_ReturnsTrue()
		{
			CreatePackage();
			fakePackage.RequireLicenseAcceptance = true;
			
			Assert.IsTrue(package.RequireLicenseAcceptance);
		}
		
		[Test]
		public void Description_WrappedPackageDescriptionIsTest_ReturnsTest()
		{
			CreatePackage();
			fakePackage.Description = "Test";
			var description = package.Description;
			
			Assert.AreEqual("Test", description);
		}
		
		[Test]
		public void Summary_WrappedPackageSummaryIsTest_ReturnsTest()
		{
			CreatePackage();
			fakePackage.Summary = "Test";
			var summary = package.Summary;
			
			Assert.AreEqual("Test", summary);
		}
		
		[Test]
		public void Language_WrappedPackageLanguageIsTest_ReturnsTest()
		{
			CreatePackage();
			fakePackage.Language = "Test";
			var language = package.Language;
			
			Assert.AreEqual("Test", language);
		}
		
		[Test]
		public void Tags_WrappedPackageTagsIsTest_ReturnsTest()
		{
			CreatePackage();
			fakePackage.Tags = "Test";
			var tags = package.Tags;
			
			Assert.AreEqual("Test", tags);
		}
		
		[Test]
		public void FrameworkAssemblies_WrappedPackageHasOneFrameworkAssembly_ReturnsOneFrameworkAssembly()
		{
			CreatePackage();
			fakePackage.FrameworkAssembliesList.Add(new FrameworkAssemblyReference("System.Xml"));
			
			var assemblies = package.FrameworkAssemblies;
			var expectedAssemblies = fakePackage.FrameworkAssemblies;
			
			CollectionAssert.AreEqual(expectedAssemblies, assemblies);
		}
		
		[Test]
		public void Dependencies_WrappedPackageHasOneDependency_ReturnsOneDependency()
		{
			CreatePackage();
			fakePackage.DependenciesList.Add(new PackageDependency("Test"));
			
			var dependencies = package.Dependencies;
			var expectedDependencies = fakePackage.Dependencies;
			
			CollectionAssert.AreEqual(expectedDependencies, dependencies);
		}
		
		[Test]
		public void GetFiles_WrappedPackageHasOneFile_ReturnsOneFile()
		{
			CreatePackage();
			fakePackage.FilesList.Add(new PhysicalPackageFile());
			
			var files = package.GetFiles();
			var expectedFiles = fakePackage.FilesList;
			
			CollectionAssert.AreEqual(expectedFiles, files);
		}
		
		[Test]
		public void DownloadCount_WrappedPackageDownloadCountIsTen_ReturnsTen()
		{
			CreatePackage();
			fakePackage.DownloadCount = 10;
			var count = package.DownloadCount;
			
			Assert.AreEqual(10, count);
		}
		
		[Test]
		public void RatingsCount_WrappedPackageRatingsCountIsTen_ReturnsTen()
		{
			CreatePackage();
			fakePackage.RatingsCount = 10;
			var count = package.RatingsCount;
			
			Assert.AreEqual(10, count);
		}
		
		[Test]
		public void Rating_WrappedPackageRatingIsFive_ReturnsFive()
		{
			CreatePackage();
			fakePackage.Rating = 5.0;
			var rating = package.Rating;
			
			Assert.AreEqual(5.0, rating);
		}
		
		[Test]
		public void GetStream_WrappedPackageHasStream_ReturnsWrappedPackageStream()
		{
			CreatePackage();
			var expectedStream = new MemoryStream();
			fakePackage.Stream = expectedStream;
			
			var stream = package.GetStream();
			
			Assert.AreEqual(expectedStream, stream);
		}
		
		[Test]
		public void HasDependencies_WrappedPackageHasNoDependencies_ReturnsFalse()
		{
			CreatePackage();
			fakePackage.DependenciesList.Clear();
			bool result = package.HasDependencies;
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasDependencies_WrappedPackageHasOneDependency_ReturnsTrue()
		{
			CreatePackage();
			fakePackage.DependenciesList.Add(new PackageDependency("Test"));
			bool result = package.HasDependencies;
			
			Assert.IsTrue(result);
		}
	}
}
