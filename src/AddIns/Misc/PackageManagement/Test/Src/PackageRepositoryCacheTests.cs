// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class PackageRepositoryCacheTests
	{
		PackageRepositoryCache cache;
		FakePackageRepositoryFactory fakePackageRepositoryFactory;
		PackageSource nuGetPackageSource;
		
		void CreateCache()
		{
			nuGetPackageSource = new PackageSource("http://nuget.org", "NuGet");
			fakePackageRepositoryFactory = new FakePackageRepositoryFactory();
			cache = new PackageRepositoryCache(fakePackageRepositoryFactory);
		}
		
		[Test]
		public void CreateRepository_CacheCastToISharpDevelopPackageRepositoryFactory_CreatesPackageRepositoryUsingPackageRepositoryFactoryPassedInConstructor()
		{
			CreateCache();
			var factory = cache as ISharpDevelopPackageRepositoryFactory;
			IPackageRepository repository = factory.CreateRepository(nuGetPackageSource);
			
			Assert.AreEqual(fakePackageRepositoryFactory.FakePackageRepository, repository);
		}
		
		[Test]
		public void CreateRepository_PackageSourcePassed_PackageSourceUsedToCreateRepository()
		{
			CreateCache();
			cache.CreateRepository(nuGetPackageSource);
			
			var actualPackageSource = fakePackageRepositoryFactory.FirstPackageSourcePassedToCreateRepository;
			Assert.AreEqual(nuGetPackageSource, actualPackageSource);
		}
		
		[Test]
		public void CreateRepository_RepositoryAlreadyCreatedForPackageSource_NoRepositoryCreated()
		{
			CreateCache();
			cache.CreateRepository(nuGetPackageSource);
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			cache.CreateRepository(nuGetPackageSource);
			
			Assert.AreEqual(0, fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Count);
		}
		
		[Test]
		public void CreateRepository_RepositoryAlreadyCreatedForPackageSource_RepositoryOriginallyCreatedIsReturned()
		{
			CreateCache();
			IPackageRepository originallyCreatedRepository = cache.CreateRepository(nuGetPackageSource);
			fakePackageRepositoryFactory.PackageSourcesPassedToCreateRepository.Clear();
			
			IPackageRepository repository = cache.CreateRepository(nuGetPackageSource);
			
			Assert.AreSame(originallyCreatedRepository, repository);
		}
		
		[Test]
		public void CreatedSharedRepository_PathResolverPassed_PathResolverUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakePackagePathResolver resolver = new FakePackagePathResolver();
			FakeSharedPackageRepository repository = cache.CreateSharedRepository(resolver, null) as FakeSharedPackageRepository;
			
			Assert.AreEqual(resolver, repository.PackagePathResolverPassedToConstructor);
		}
		
		[Test]
		public void CreatedSharedRepository_FileSystemPassed_FileSystemUsedToCreatedSharedRepository()
		{
			CreateCache();
			FakeFileSystem fileSystem = new FakeFileSystem();
			FakeSharedPackageRepository repository = cache.CreateSharedRepository(null, fileSystem) as FakeSharedPackageRepository;
			
			Assert.AreEqual(fileSystem, repository.FileSystemPassedToConstructor);
		}
	}
}
