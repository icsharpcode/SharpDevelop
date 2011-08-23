// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableGetProjectCmdlet : GetProjectCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakeCommandRuntime FakeCommandRuntime;
		public FakePackageManagementSolution FakeSolution;
		
		public TestableGetProjectCmdlet()
			: this(
				new FakePackageManagementConsoleHost(),
				new FakeCommandRuntime(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableGetProjectCmdlet(
			FakePackageManagementConsoleHost consoleHost,
			FakeCommandRuntime commandRuntime,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(consoleHost, cmdletTerminatingError)
		{
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCommandRuntime = commandRuntime;
			this.CommandRuntime = commandRuntime;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
			this.FakeSolution = consoleHost.FakeSolution;
		}
		
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
