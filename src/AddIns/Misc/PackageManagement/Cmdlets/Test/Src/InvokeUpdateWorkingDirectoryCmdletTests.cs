// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InvokeUpdateWorkingDirectoryCmdletTests
	{
		TestableInvokeUpdateWorkingDirectoryCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementProjectService fakeProjectService;
		
		void CreateCmdlet(string solutionFileName = @"d:\projects\MyProject\MyProject.sln")
		{
			cmdlet = new TestableInvokeUpdateWorkingDirectoryCmdlet(solutionFileName);
			fakeProjectService = cmdlet.FakeProjectService;
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
		
		[Test]
		public void ProcessRecord_SolutionIsOpen_PowerShellWorkingDirectoryIsSetToSolutionDirectory()
		{
			string solutionFileName = @"d:\projects\MySolution\MySolution.sln";
			CreateCmdlet(solutionFileName);
			RunCmdlet();
			
			string commandExecuted = cmdlet.ScriptPassedToInvokeScript;
			
			string expectedCommandExecuted = @"Set-Location 'd:\projects\MySolution'";
			
			Assert.AreEqual(expectedCommandExecuted, commandExecuted);
		}

		[Test]
		public void ProcessRecord_SolutionIsNotOpen_PowerShellWorkingDirectoryIsSetUserProfileFolder()
		{
			CreateCmdlet();
			fakeProjectService.OpenSolution = null;
			RunCmdlet();
			
			string commandExecuted = cmdlet.ScriptPassedToInvokeScript;
			
			string expectedCommandExecuted = @"Set-Location $env:USERPROFILE";
			
			Assert.AreEqual(expectedCommandExecuted, commandExecuted);
		}
	}
}
