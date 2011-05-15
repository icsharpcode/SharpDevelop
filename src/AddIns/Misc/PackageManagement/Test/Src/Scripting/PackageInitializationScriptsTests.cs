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
		FakePackageScriptFactoryWithPredefinedPackageScripts fakeScriptFactoryWithPredefinedPackageScripts;
		
		void CreateScripts()
		{
			fakeScriptFactory = new FakePackageScriptFactory();
			CreateScripts(fakeScriptFactory);
		}
		
		void CreateScripts(IPackageScriptFactory scriptFactory)
		{
			fakeSolutionPackageRepository = new FakeSolutionPackageRepository();
			scripts = new PackageInitializationScripts(fakeSolutionPackageRepository, scriptFactory);
		}
		
		FakePackage AddPackageToRepository(string packageId)
		{
			var package = new FakePackage(packageId);
			fakeSolutionPackageRepository.FakePackages.Add(package);
			return package;
		}
		
		void CreateScriptsUsingScriptFactoryWithPredefinedScripts()
		{
			fakeScriptFactoryWithPredefinedPackageScripts = new FakePackageScriptFactoryWithPredefinedPackageScripts();
			CreateScripts(fakeScriptFactoryWithPredefinedPackageScripts);
		}
		
		void CreateScriptsWithTwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst()
		{
			CreateScriptsUsingScriptFactoryWithPredefinedScripts();
			AddPackageToRepository("A");
			AddPackageToRepository("B");
			
			var scriptForPackageA = fakeScriptFactoryWithPredefinedPackageScripts.AddFakeInitializationScript();
			scriptForPackageA.ExistsReturnValue = false;
			
			var scriptForPackageB = fakeScriptFactoryWithPredefinedPackageScripts.AddFakeInitializationScript();
			scriptForPackageB.ExistsReturnValue = true;			
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
		
		[Test]
		public void Any_OnePackageInRepositoryAndScriptFileExists_ReturnsTrue()
		{
			CreateScripts();
			AddPackageToRepository("A");
			fakeScriptFactory.ScriptFileExistsReturnValue = true;
			
			bool result = scripts.Any();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Any_OnePackageInRepositoryAndScriptFileDoesNotExist_ReturnsFalse()
		{
			CreateScripts();
			AddPackageToRepository("A");
			fakeScriptFactory.ScriptFileExistsReturnValue = false;
			
			bool result = scripts.Any();
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void Any_TwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst_ReturnsTrue()
		{
			CreateScriptsWithTwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst();
			
			bool result = scripts.Any();
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void Run_TwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst_FirstScriptNotExecuted()
		{
			CreateScriptsWithTwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst();
			FakePackageScript firstScript = fakeScriptFactoryWithPredefinedPackageScripts.FakeInitializeScripts[0];
			scripts.Run();
			
			bool executed = firstScript.IsExecuted;
			
			Assert.IsFalse(executed);
		}
	}
}
