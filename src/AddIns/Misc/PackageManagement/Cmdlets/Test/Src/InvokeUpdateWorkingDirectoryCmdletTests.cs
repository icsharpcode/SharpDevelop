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
