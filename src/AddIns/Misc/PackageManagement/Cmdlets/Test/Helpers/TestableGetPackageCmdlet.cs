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
		public FakePackageManagementSolution FakeSolution;
		public FakeRegisteredPackageRepositories FakeRegisteredPackageRepositories;
		public FakeCommandRuntime FakeCommandRuntime;
		public List<IPackage> PackagesPassedToWriteObject = new List<IPackage>();
		public FakePackageManagementConsoleHost FakePackageManagementConsoleHost;
		public FakeCmdletTerminatingError FakeCmdletTerminatingError;
		
		public TestableGetPackageCmdlet()
			: this(
				new FakePackageManagementSolution(),
				new FakeRegisteredPackageRepositories(),
				new FakePackageManagementConsoleHost(),
				new FakeCommandRuntime(),
				new FakeCmdletTerminatingError())
		{
		}
		
		public TestableGetPackageCmdlet(
			FakePackageManagementSolution solution,
			FakeRegisteredPackageRepositories registeredPackageRepositories,
			FakePackageManagementConsoleHost consoleHost,
			FakeCommandRuntime commandRuntime,
			FakeCmdletTerminatingError terminatingError)
			: base(registeredPackageRepositories, solution, consoleHost, terminatingError)
		{
			this.FakeSolution = solution;
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
