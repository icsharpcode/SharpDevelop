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
		FakePackageScriptSession fakeSession;
		FakePackageScriptFactoryWithPredefinedPackageScripts fakeScriptFactoryWithPredefinedPackageScripts;
		
		void CreateScripts()
		{
			fakeScriptFactory = new FakePackageScriptFactory();
			CreateScripts(fakeScriptFactory);
		}
		
		void CreateScripts(IPackageScriptFactory scriptFactory)
		{
			fakeSolutionPackageRepository = new FakeSolutionPackageRepository();
			fakeSession = new FakePackageScriptSession();
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
		public void Run_OnePackageInRepository_OnePackageScriptIsRun()
		{
			CreateScripts();
			AddPackageToRepository("Test");
			scripts.Run(fakeSession);
			
			IPackageScriptSession session = fakeScriptFactory.FirstPackageInitializeScriptCreated.SessionPassedToRun;
			FakePackageScriptSession expectedSession = fakeSession;
			
			Assert.AreEqual(expectedSession, session);
		}
		
		[Test]
		public void Run_OnePackageInRepository_PackageInitScriptCreatedForPackage()
		{
			CreateScripts();
			AddPackageToRepository("Test");
			string expectedDirectory = @"d:\projects\myproject\packages\Test.1.0";
			fakeSolutionPackageRepository.InstallPathToReturn = expectedDirectory;
			
			scripts.Run(fakeSession);
			
			string actualDirectory = fakeScriptFactory.FirstPackageInstallDirectoryPassed;
			
			Assert.AreEqual(expectedDirectory, actualDirectory);
		}
		
		[Test]
		public void Run_OnePackageInRepository_PackageUsedToDetermineInstallPath()
		{
			CreateScripts();
			FakePackage package = AddPackageToRepository("Test");			
			scripts.Run(fakeSession);
			
			IPackage actualPackage = fakeSolutionPackageRepository.PackagePassedToGetInstallPath;
			
			Assert.AreEqual(package, actualPackage);
		}
		
		[Test]
		public void Run_TwoPackagesInRepository_SecondScriptIsRun()
		{
			CreateScripts();
			AddPackageToRepository("A");
			AddPackageToRepository("B");
			scripts.Run(fakeSession);
			
			IPackageScriptSession session = fakeScriptFactory.FakePackageInitializeScriptsCreated[1].SessionPassedToRun;
			FakePackageScriptSession expectedSession = fakeSession;
			
			Assert.AreEqual(expectedSession, session);
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
		public void Run_TwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst_FirstScriptNotRun()
		{
			CreateScriptsWithTwoPackagesInRepositoryAndLastPackageScriptFileExistsButNotFirst();
			FakePackageScript firstScript = fakeScriptFactoryWithPredefinedPackageScripts.FakeInitializeScripts[0];
			scripts.Run(fakeSession);
			
			bool run = firstScript.IsRun;
			
			Assert.IsFalse(run);
		}
		
		[Test]
		public void Run_OnePackageInRepository_PackageScriptSetsPackageInSession()
		{
			CreateScripts();
			FakePackage expectedPackage = AddPackageToRepository("Test");
			scripts.Run(fakeSession);
			
			var package = fakeScriptFactory.FirstPackageInitializeScriptCreated.Package;
			
			Assert.AreEqual(expectedPackage, package);
		}
	}
}
