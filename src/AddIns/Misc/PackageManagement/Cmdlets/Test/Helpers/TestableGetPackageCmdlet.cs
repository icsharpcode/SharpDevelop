// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
