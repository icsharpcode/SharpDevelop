// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableInvokeUpdateWorkingDirectoryCmdlet : InvokeUpdateWorkingDirectoryCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakePackageManagementProjectService FakeProjectService;
		public Solution Solution;
		
		public TestableInvokeUpdateWorkingDirectoryCmdlet()
			: this(
				new FakePackageManagementProjectService(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableInvokeUpdateWorkingDirectoryCmdlet(
			FakePackageManagementProjectService projectService,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(projectService, consoleHost, cmdletTerminatingError)
		{
			this.FakeProjectService = projectService;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
			
			Solution = new Solution(new MockProjectChangeWatcher());
			Solution.FileName = @"d:\projects\MyProject\MyProject.sln";
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
