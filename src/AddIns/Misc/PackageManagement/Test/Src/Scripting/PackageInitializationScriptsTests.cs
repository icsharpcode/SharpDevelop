// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInitializationScriptsTests
	{
		PackageInitializationScripts scripts;
		FakePackageScriptFactory fakeScriptFactory;
		FakeSolutionPackageRepository fakeSolutionPackageRepository;
		
		void CreateScripts()
		{
			fakeSolutionPackageRepository = new FakeSolutionPackageRepository();
			fakeScriptFactory = new FakePackageScriptFactory();
			scripts = new PackageInitializationScripts(fakeSolutionPackageRepository, fakeScriptFactory);
		}
		
		FakePackage AddPackageToRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			fakeSolutionPackageRepository.FakePackages.Add(package);
			return package;
		}
		
		[Test]
		public void Run_OnePackageInRepository_OnePackageScriptExecuted()
		{
			CreateScripts();
			AddPackageToRepository("Test");
			scripts.Run();
			
			bool executed = fakeScriptFactory.FirstPackageInitializeScriptCreated.IsExecuted;
			
			Assert.IsTrue(executed);
		}
		
		[Test]
		public void Run_OnePackageInRepository_PackageInitScriptCreatedForPackage()
		{
			CreateScripts();
			AddPackageToRepository("Test");
			string expectedDirectory = @"d:\projects\myproject\packages\Test.1.0";
			fakeSolutionPackageRepository.InstallPathToReturn = expectedDirectory;
			
			scripts.Run();
			
			string actualDirectory = fakeScriptFactory.FirstPackageInstallDirectoryPassed;
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void Run_OnePackageInRepository_PackageUsedToDetermineInstallPath()
		{
			CreateScripts();
			FakePackage package = AddPackageToRepository("Test");			
			scripts.Run();
			
			IPackage actualPackage = fakeSolutionPackageRepository.PackagePassedToGetInstallPath;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void Run_TwoPackagesInRepository_SecondScriptIsExecuted()
		{
			CreateScripts();
			AddPackageToRepository("A");
			AddPackageToRepository("B");
			scripts.Run();
			
			bool executed = fakeScriptFactory.FakePackageInitializeScriptsCreated[1].IsExecuted;
			
			Assert.IsTrue(executed);
		}
	}
}
