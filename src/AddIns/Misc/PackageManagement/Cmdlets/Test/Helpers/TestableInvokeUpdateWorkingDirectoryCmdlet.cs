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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableInvokeUpdateWorkingDirectoryCmdlet : InvokeUpdateWorkingDirectoryCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakePackageManagementProjectService FakeProjectService;
		public ISolution Solution;
		
		public TestableInvokeUpdateWorkingDirectoryCmdlet(string solutionFileName)
			: this(
				solutionFileName,
				new FakePackageManagementProjectService(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableInvokeUpdateWorkingDirectoryCmdlet(
			string solutionFileName,
			FakePackageManagementProjectService projectService,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(projectService, consoleHost, cmdletTerminatingError)
		{
			this.FakeProjectService = projectService;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
			
			Solution = MockRepository.GenerateStub<ISolution>();
			var fileName = new FileName(solutionFileName);
			Solution.Stub(s => s.Directory).Return(fileName.GetParentDirectory());
			Solution.Stub(s => s.FileName).Return(fileName);
			projectService.OpenSolution = Solution;
		}
		
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
			
		public string ScriptPassedToInvokeScript;
		
		public override void InvokeScript(string script)
		{
			ScriptPassedToInvokeScript = script;
		}
	}
}
