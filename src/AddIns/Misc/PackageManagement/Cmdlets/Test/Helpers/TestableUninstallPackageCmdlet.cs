// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableUninstallPackageCmdlet : UninstallPackageCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakePackageManagementSolution FakeSolution;
		
		public TestableUninstallPackageCmdlet()
			: this(
				new FakePackageManagementSolution(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableUninstallPackageCmdlet(
			FakePackageManagementSolution solution,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(solution, consoleHost, cmdletTerminatingError)
		{
			this.FakeSolution = solution;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
		}
		
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
