// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests
{
	public abstract class CmdletTestsBase
	{
		protected FakePackageManagementConsoleHost fakeConsoleHost;
		
		protected TestableProject AddDefaultProjectToConsoleHost()
		{
			return fakeConsoleHost.AddFakeDefaultProject();
		}
		
		protected PackageSource AddPackageSourceToConsoleHost()
		{
			return fakeConsoleHost.AddTestPackageSource();
		}
	}
}
