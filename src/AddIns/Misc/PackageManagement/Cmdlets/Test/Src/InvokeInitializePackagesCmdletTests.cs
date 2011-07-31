// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.PackageManagement.Scripting;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using PackageManagement.Cmdlets.Tests.Helpers;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	[TestFixture]
	public class InvokeInitializePackagesCmdletTests
	{
		TestableInvokeInitializePackagesCmdlet cmdlet;
		FakeCmdletTerminatingError fakeTerminatingError;
		FakePackageManagementProjectService fakeProjectService;
		FakePackageInitializationScriptsFactory scriptsFactory;
		
		void CreateCmdlet()
		{
			cmdlet = new TestableInvokeInitializePackagesCmdlet();
			fakeProjectService = cmdlet.FakeProjectService;
			fakeTerminatingError = cmdlet.FakeCmdletTerminatingError;
			scriptsFactory = cmdlet.FakeScriptsFactory;
		}
		
		void RunCmdlet()
		{
			cmdlet.CallProcessRecord();
		}
	
		void SolutionHasPackageInitializationScripts()
		{
			scriptsFactory.FakePackageInitializationScripts.AnyReturnValue = true;
		}
		
		void SolutionHasNoPackageInitializationScripts()
		{
			scriptsFactory.FakePackageInitializationScripts.AnyReturnValue = false;
		}
		
		[Test]
		public void ProcessRecord_SolutionHasPackageInitializationScripts_PackageInitializationScriptsAreRun()
		{
			CreateCmdlet();
			SolutionHasPackageInitializationScripts();
			RunCmdlet();
			
			IPackageScriptSession session = scriptsFactory.FakePackageInitializationScripts.SessionPassedToRun;
			
			Assert.AreEqual(cmdlet, session);
		}
		
		[Test]
		public void ProcessRecord_SolutionHasNoPackageInitializationScripts_PackageInitializationScriptsAreNotRun()
		{
			CreateCmdlet();
			SolutionHasNoPackageInitializationScripts();
			RunCmdlet();
			
			IPackageScriptSession session = scriptsFactory.FakePackageInitializationScripts.SessionPassedToRun;
			
			Assert.IsNull(session);
		}
		
		[Test]
		public void ProcessRecord_SolutionIsOpen_SolutionUsedToCreatePackageInitializationScripts()
		{
			CreateCmdlet();
			RunCmdlet();
			
			Solution expectedSolution = fakeProjectService.OpenSolution;
			Solution actualSolution = scriptsFactory.SolutionPassedToCreatePackageInitializationScripts;
			
			Assert.AreEqual(expectedSolution, actualSolution);
		}
		
		[Test]
		public void ProcessRecord_SolutionHasPackageInitializationScripts_PowerShellWorkingDirectoryIsUpdated()
		{
			CreateCmdlet();
			fakeProjectService.OpenSolution = null;
			RunCmdlet();
			
			string commandExecuted = cmdlet.ScriptPassedToInvokeScript;
			
			string expectedCommandExecuted = @"Invoke-UpdateWorkingDirectory";
			
			Assert.AreEqual(expectedCommandExecuted, commandExecuted);
		}
	}		
}
