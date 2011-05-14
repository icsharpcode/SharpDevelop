// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.Scripting
{
	[TestFixture]
	public class PackageInitializationScriptsRunnerForOpenedSolutionTests
	{
		FakePackageInitializationScriptsFactory fakeScriptsFactory;
		FakePackageManagementProjectService fakeProjectService;
		PackageInitializationScriptsRunnerForOpenedSolution runner;
		
		void CreateRunner()
		{
			fakeScriptsFactory = new FakePackageInitializationScriptsFactory();
			fakeProjectService = new FakePackageManagementProjectService();
			runner = new PackageInitializationScriptsRunnerForOpenedSolution(fakeProjectService, fakeScriptsFactory);
		}
		
		Solution OpenSolution()
		{
			var solution = new Solution();
			solution.FileName = @"d:\projects\myprojects\test.csproj";
			fakeProjectService.FireSolutionLoadedEvent(solution);
			return solution;
		}
		
		[Test]
		public void Instance_SolutionIsOpened_PackageInitializationScriptsAreExecuted()
		{
			CreateRunner();
			OpenSolution();
			
			bool run = fakeScriptsFactory.FakePackageInitializationScripts.IsRunCalled;
			
			Assert.IsTrue(run);
		}
		
		[Test]
		public void Instance_SolutionIsOpened_PackageInitializationScriptsCreatedUsingSolution()
		{
			CreateRunner();
			var expectedSolution = OpenSolution();
			
			var actualSolution = fakeScriptsFactory.SolutionPassedToCreatePackageInitializationScripts;
			
			Assert.AreEqual(expectedSolution, actualSolution);
		}
	}
}
