// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class SharpDevelopPackageManagerFactoryTests
	{
		SharpDevelopPackageManagerFactory factory;
		IPackageManager packageManager;
		FakePackageRepository fakePackageRepository;
		MSBuildBasedProject testProject;
		PackageManagementOptions options;
		FakePackageRepositoryFactory fakePackageRepositoryFactory;
		FakeProjectSystemFactory fakeProjectSystemFactory;
		
		void CreateFactory()
		{
			options = new PackageManagementOptions(new Properties());
			fakePackageRepositoryFactory = new FakePackageRepositoryFactory();
			fakeProjectSystemFactory = new FakeProjectSystemFactory();
			factory = new SharpDevelopPackageManagerFactory(fakePackageRepositoryFactory, fakeProjectSystemFactory, options);
		}
		
		void CreateTestProject()
		{
			testProject = ProjectHelper.CreateTestProject();
		}
		
		void CreatePackageManager()
		{
			fakePackageRepository = new FakePackageRepository();
			packageManager = factory.CreatePackageManager(fakePackageRepository, testProject);
		}
		
		[Test]
		public void CreatePackageManager_ProjectAndSolutionHaveDifferentFolders_PackageManagerLocalRepositoryIsSharedRepository()
		{
			CreateFactory();
			CreateTestProject();
			CreatePackageManager();
			ISharedPackageRepository sharedRepository = packageManager.LocalRepository as ISharedPackageRepository;
			
			Assert.IsNotNull(sharedRepository);
		}
		
		[Test]
		public void CreatePackageManager_PackagesSolutionFolderDefinedInOptions_SharedLocalRepositoryFileSystemRootIsSolutionFolder()
		{
			CreateFactory();
			CreateTestProject();
			testProject.ParentSolution.FileName = @"c:\projects\MyProject\MySolution.sln";
			options.PackagesDirectory = "MyPackages";
			CreatePackageManager();
			
			FakeSharedPackageRepository sharedRepository = packageManager.LocalRepository as FakeSharedPackageRepository;
			
			string expectedRoot = @"c:\projects\MyProject\MyPackages";
			Assert.AreEqual(expectedRoot, sharedRepository.FileSystemPassedToConstructor.Root);
		}
		
		[Test]
		public void CreatePackageManager_PackagesSolutionFolderDefinedInOptions_SharedLocalRepositoryPackagePathResolverCreatedWithPackagesFolderInsideSolutionFolder()
		{
			CreateFactory();
			CreateTestProject();
			testProject.ParentSolution.FileName = @"c:\projects\MyProject\MySolution.sln";
			options.PackagesDirectory = "MyPackages";
			CreatePackageManager();
			
			FakeSharedPackageRepository sharedRepository = packageManager.LocalRepository as FakeSharedPackageRepository;
			
			FakePackage package = new FakePackage("Test.Package");
			package.Version = new Version(1, 0, 0, 0);
			string expectedDirectory = @"c:\projects\MyProject\MyPackages\Test.Package.1.0.0.0";
			string actualDirectory = sharedRepository.PackagePathResolverPassedToConstructor.GetInstallPath(package);
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void CreatePackageManager_PackagesSolutionFolderDefinedInOptions_LocalRepositoryFileSystemIsPackageManagerFileSystem()
		{
			CreateFactory();
			CreateTestProject();
			CreatePackageManager();
			
			FakeSharedPackageRepository sharedRepository = packageManager.LocalRepository as FakeSharedPackageRepository;
			
			Assert.AreEqual(packageManager.FileSystem, sharedRepository.FileSystemPassedToConstructor);
		}
		
		[Test]
		public void CreatePackageManager_PackagesSolutionFolderDefinedInOptions_PackageManagerPathResolverUsesPackagesFolderInsideSolutionFolder()
		{
			CreateFactory();
			CreateTestProject();
			testProject.ParentSolution.FileName = @"c:\projects\MyProject\MySolution.sln";
			options.PackagesDirectory = "packages";
			CreatePackageManager();
			
			FakePackage package = new FakePackage();
			package.Id = "TestPackage";
			package.Version = new Version(1, 0, 0, 0);
			
			string expectedDirectory = @"c:\projects\MyProject\packages\TestPackage.1.0.0.0";
			
			SharpDevelopPackageManager sharpDevelopPackageManager = packageManager as SharpDevelopPackageManager;
			string actualDirectory = sharpDevelopPackageManager.PathResolver.GetInstallPath(package);
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
	}
}
