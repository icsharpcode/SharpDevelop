// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestablePackageManagementCmdlet : PackageManagementCmdlet
	{
		public TestablePackageManagementCmdlet(FakeCmdletTerminatingError terminatingError)
			: base(
				new FakePackageManagementService(),
				new FakePackageManagementConsoleHost(),
				terminatingError)
		{
			this.CommandRuntime = new FakeCommandRuntime();
		}
		
		public void CallThrowProjectNotOpenTerminatingError()
		{
			base.ThrowProjectNotOpenTerminatingError();
		}
	}
}
