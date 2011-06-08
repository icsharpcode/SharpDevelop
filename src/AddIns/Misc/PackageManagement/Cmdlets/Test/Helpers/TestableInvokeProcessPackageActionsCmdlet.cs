// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Scripting;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableInvokeProcessPackageActionsCmdlet : InvokeProcessPackageActionsCmdlet
	{
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public PackageActionsToRun ActionsToRun;
		
		public TestableInvokeProcessPackageActionsCmdlet()
			: this(
				new PackageActionsToRun(),
				new FakePackageManagementConsoleHost(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableInvokeProcessPackageActionsCmdlet(
			PackageActionsToRun actionsToRun,
			FakePackageManagementConsoleHost consoleHost,
			FakeCmdletTerminatingError cmdletTerminatingError)
			: base(actionsToRun, consoleHost, cmdletTerminatingError)
		{
			this.ActionsToRun = actionsToRun;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCmdletTerminatingError = cmdletTerminatingError;
		}
		
		public void CallProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
