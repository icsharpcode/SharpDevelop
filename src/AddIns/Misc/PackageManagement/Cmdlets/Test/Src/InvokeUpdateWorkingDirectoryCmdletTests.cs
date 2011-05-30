// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InvokeUpdateWorkingDirectoryCmdletTests
	{
		TestableInvokeUpdateWorkingDirectoryCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementProjectService fakeProjectService;
		
		void CreateCmdlet()
		{
			cmdlet = new TestableInvokeUpdateWorkingDirectoryCmdlet();
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
			CreateCmdlet();
			fakeProjectService.OpenSolution.FileName = @"d:\projects\MySolution\MySolution.sln";
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
