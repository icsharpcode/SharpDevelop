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
		
		public TestableUninstallPackageCmdlet()
			: this(
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableUninstallPackageCmdlet(
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(consoleHost, cmdletTerminatingError)
		{
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
		}
		
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
