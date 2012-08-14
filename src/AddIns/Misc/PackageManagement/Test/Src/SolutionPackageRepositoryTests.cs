// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SolutionPackageRepositoryTests
	{
		SolutionPackageRepository repository;
		TestablePackageManagementOptions options;
		Solution solution;
		FakePackageRepositoryFactory fakeRepositoryFactory;
		FakeSharedPackageRepository fakeSharedRepository;
		
		void CreateSolution(string fileName)
		{
			solution = new Solution(new MockProjectChangeWatcher());
			solution.FileName = fileName;
		}
		
		void CreateFakeRepositoryFactory()
		{
			fakeRepositoryFactory = new FakePackageRepositoryFactory();
			fakeSharedRepository = fakeRepositoryFactory.FakeSharedRepository;
		}
		
		void CreateOptions()
		{
			options = new TestablePackageManagementOptions();
		}
		
		void CreateRepository(Solution solution, TestablePackageManagementOptions options)
		{
			CreateFakeRepositoryFactory();
			repository = new SolutionPackageRepository(solution, fakeRepositoryFactory, options);
		}
		
		void CreateRepository(Solution solution)
		{
			CreateOptions();
			CreateRepository(solution, options);
		}
		
		void CreateRepository()
		{
			solution = new Solution(new MockProjectChangeWatcher());
			solution.FileName = @"d:\projects\test\myproject\myproject.sln";
			CreateRepository(solution);
		}
		
		FakePackage AddPackageToSharedRepository(string packageId)
		{
			FakeSharedPackageRepository sharedRepository = fakeRepositoryFactory.FakeSharedRepository;
			return sharedRepository.AddFakePackage(packageId);
		}
		
		FakePackage AddPackageToSharedRepository(string packageId, string version)
		{
			FakeSharedPackageRepository sharedRepository = fakeRepositoryFactory.FakeSharedRepository;
			return sharedRepository.AddFakePackageWithVersion(packageId, version);
		}
		
		[Test]
		public void GetInstallPath_GetInstallPathForPackage_ReturnsPackagePathInsideSolutionPackagesRepository()
		{
			CreateSolution(@"d:\projects\Test\MySolution\MyProject.sln");
			CreateOptions();
			options.PackagesDirectory = "MyPackages";
			CreateRepository(solution, options);
			
			var package = FakePackage.CreatePackageWithVersion("MyPackage", "1.0.1.40");
			
			string installPath = repository.GetInstallPath(package);
			
			string expectedInstallPath = 
				@"d:\projects\Test\MySolution\MyPackages\MyPackage.1.0.1.40";
			
			Assert.AreEqual(expectedInstallPath, installPath);
		}
		
		[Test]
		public void GetPackagesByDependencyOrder_OnePackageInSharedRepository_ReturnsOnePackage()
		{
			CreateRepository();
			AddPackageToSharedRepository("Test");
			
			List<FakePackage> expectedPackages = fakeSharedRepository.FakePackages;
			
			List<IPackage> actualPackages = repository.GetPackagesByDependencyOrder().ToList();
			
			PackageCollectionAssert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void GetPackagesByDependencyOrder_OnePackageInSharedRepository_SharedRepositoryCreatedWithPathResolverForSolutionPackagesFolder()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			FakePackage package = AddPackageToSharedRepository("Test", "1.0");
						
			List<IPackage> actualPackages = repository.GetPackagesByDependencyOrder().ToList();
			
			IPackagePathResolver pathResolver = fakeRepositoryFactory.PathResolverPassedToCreateSharedRepository;
			string installPath = pathResolver.GetInstallPath(package);
			
			string expectedInstallPath = @"d:\projects\myproject\packages\Test.1.0";
			
			Assert.AreEqual(expectedInstallPath, installPath);
		}
		
		[Test]
		public void Constructor_CreateInstance_SharedRepositoryCreatedWithFileSystemForSolutionPackagesFolder()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			
			IFileSystem fileSystem = fakeRepositoryFactory.FileSystemPassedToCreateSharedRepository;
			string rootPath = fileSystem.Root;
			
			string expectedRootPath = @"d:\projects\myproject\packages";
			
			Assert.AreEqual(expectedRootPath, rootPath);
		}
		
		[Test]
		public void Constructor_CreateInstance_SharedRepositoryCreatedWithConfigSettingsFileSystemForSolutionNuGetFolder()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			
			IFileSystem fileSystem = fakeRepositoryFactory.ConfigSettingsFileSystemPassedToCreateSharedRepository;
			string rootPath = fileSystem.Root;
			
			string expectedRootPath = @"d:\projects\myproject\.nuget";
			
			Assert.AreEqual(expectedRootPath, rootPath);
		}
		
		[Test]
		public void GetPackagesByDependencyOrder_TwoPackagesInSharedRepositoryFirstPackageDependsOnSecond_ReturnsSecondPackageFirst()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			FakePackage firstPackage = AddPackageToSharedRepository("First");
			firstPackage.AddDependency("Second");
			FakePackage secondPackage = AddPackageToSharedRepository("Second");
			
			List<IPackage> actualPackages = repository.GetPackagesByDependencyOrder().ToList();
			
			var expectedPackages = new IPackage[] {
				secondPackage,
				firstPackage
			};
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void GetPackagesByReverseDependencyOrder_TwoPackagesInSharedRepositorySecondPackageDependsOnFirst_ReturnsSecondPackageFirst()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			FakePackage firstPackage = AddPackageToSharedRepository("First");
			FakePackage secondPackage = AddPackageToSharedRepository("Second");
			secondPackage.AddDependency("First");
						
			List<IPackage> actualPackages = repository.GetPackagesByReverseDependencyOrder().ToList();
			
			var expectedPackages = new IPackage[] {
				secondPackage,
				firstPackage
			};
			
			Assert.AreEqual(expectedPackages, actualPackages);
		}
		
		[Test]
		public void IsInstalled_PackageIsInSharedRepository_ReturnsTrue()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			FakePackage firstPackage = AddPackageToSharedRepository("First");
			
			bool installed = repository.IsInstalled(firstPackage);
			
			Assert.IsTrue(installed);
		}
		
		[Test]
		public void IsInstalled_PackageIsNotInSharedRepository_ReturnsFalse()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			FakePackage testPackage = new FakePackage("Test");
			
			bool installed = repository.IsInstalled(testPackage);
			
			Assert.IsFalse(installed);
		}
		
		[Test]
		public void GetPackages_OnePackageIsInSharedRepository_ReturnsOnePackage()
		{
			CreateSolution(@"d:\projects\myproject\myproject.sln");
			CreateRepository(solution);
			FakePackage firstPackage = AddPackageToSharedRepository("First");
			
			IQueryable<IPackage> packages = repository.GetPackages();
			
			var expectedPackages = new FakePackage[] {
				firstPackage
			};
			
			PackageCollectionAssert.AreEqual(expectedPackages, packages);
		}
	}
}
