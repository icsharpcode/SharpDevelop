// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableUpdatePackageCmdlet : UpdatePackageCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakeUpdatePackageActionsFactory FakeUpdatePackageActionsFactory;
		
		public TestableUpdatePackageCmdlet()
			: this(
				new FakeUpdatePackageActionsFactory(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableUpdatePackageCmdlet(
			FakeUpdatePackageActionsFactory updatePackageActionsFactory,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(updatePackageActionsFactory, consoleHost, cmdletTerminatingError)
		{
			this.FakeUpdatePackageActionsFactory = updatePackageActionsFactory;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
		}
				
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
