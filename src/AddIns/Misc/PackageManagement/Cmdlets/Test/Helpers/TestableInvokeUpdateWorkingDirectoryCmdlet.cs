// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
