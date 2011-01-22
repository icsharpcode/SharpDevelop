// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementProjectService : IPackageManagementProjectService
	{
		public bool IsRefreshProjectBrowserCalled;
		
		public IProject CurrentProject { get; set; }
		
		public void RefreshProjectBrowser()
		{
			IsRefreshProjectBrowserCalled = true;
		}
	}
}
