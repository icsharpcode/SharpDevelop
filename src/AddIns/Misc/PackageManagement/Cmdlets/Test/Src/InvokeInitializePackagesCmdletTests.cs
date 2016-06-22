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
			
			ISolution expectedSolution = fakeProjectService.OpenSolution;
			ISolution actualSolution = scriptsFactory.SolutionPassedToCreatePackageInitializationScripts;
			
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
