// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Helpers.Tests
{
	[TestFixture]
	public class PackageReferenceRepositoryTests
	{
		FakeSharedPackageRepository sourceRepository;
		PackageReferenceRepository repository;
		FakeFileSystem fakeFileSystem;
		PackageReferenceRepositoryHelper helper;
		
		void CreatePackageReferenceRepository()
		{
			helper = new PackageReferenceRepositoryHelper();
			fakeFileSystem = helper.FakeProjectSystem;
			sourceRepository = helper.FakeSharedSourceRepository;
			repository = new PackageReferenceRepository(fakeFileSystem, sourceRepository);
		}
		
		[Test]
		public void RegisteredIfRequired_PackageIdInConfigFileAndInSourceRepository_MethodCallWithPathToConfigFile()
		{
			CreatePackageReferenceRepository();
			
			string expectedPath = @"d:\temp";
			fakeFileSystem.PathToReturnFromGetFullPath = expectedPath;
						
			repository.RegisterIfNecessary();
			
			Assert.AreEqual(expectedPath, sourceRepository.PathPassedToRegisterRepository);
		}
	}
}
