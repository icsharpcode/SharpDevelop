// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using ICSharpCode.SharpDevelop.Project;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableInvokeInitializePackagesCmdlet : InvokeInitializePackagesCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakePackageManagementProjectService FakeProjectService;
		public FakePackageInitializationScriptsFactory FakeScriptsFactory;
		public Solution Solution;
		
		public TestableInvokeInitializePackagesCmdlet()
			: this(
				new FakePackageManagementProjectService(),
				new FakePackageInitializationScriptsFactory(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableInvokeInitializePackagesCmdlet(
			FakePackageManagementProjectService projectService,
			FakePackageInitializationScriptsFactory scriptsFactory,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(projectService, scriptsFactory, consoleHost, cmdletTerminatingError)
		{
			this.FakeProjectService = projectService;
			this.FakeScriptsFactory = scriptsFactory;
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
