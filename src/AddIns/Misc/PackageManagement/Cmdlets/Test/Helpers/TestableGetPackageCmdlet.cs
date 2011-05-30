// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using NuGet;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestableGetPackageCmdlet : GetPackageCmdlet
	{
		public FakeRegisteredPackageRepositories FakeRegisteredPackageRepositories;
		public FakeCommandRuntime FakeCommandRuntime;
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		
		public TestableGetPackageCmdlet()
			: this(
				new FakeRegisteredPackageRepositories(),
				new FakePackageManagementConsoleHost(),
				new FakeCommandRuntime(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableGetPackageCmdlet(
			FakeRegisteredPackageRepositories registeredPackageRepositories,
			FakePackageManagementConsoleHost consoleHost,
			FakeCommandRuntime commandRuntime,
			FakeCmdletTerminatingError terminatingError)
			: base(registeredPackageRepositories, consoleHost, terminatingError)
		{
			this.FakeRegisteredPackageRepositories = registeredPackageRepositories;
			this.FakePackageManagementConsoleHost = consoleHost;
			this.FakeCommandRuntime = commandRuntime;
			this.CommandRuntime = commandRuntime;
			this.FakeCmdletTerminatingError = terminatingError;
		}
		
		public void RunProcessRecord()
		{
			base.ProcessRecord();
		}
	}
}
