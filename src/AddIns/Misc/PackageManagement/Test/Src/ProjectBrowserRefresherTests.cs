// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NUnit.Framework;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class ProjectBrowserRefresherTests
	{
		ProjectBrowserRefresher projectBrowserRefresher;
		FakePackageManagementProjectService fakeProjectService;
		PackageManagementEvents packageManagementEvents;
		
		void CreateProjectBrowserRefresher()
		{
			fakeProjectService = new FakePackageManagementProjectService();
			packageManagementEvents = new PackageManagementEvents();
			projectBrowserRefresher = new ProjectBrowserRefresher(fakeProjectService, packageManagementEvents);
		}
		
		void RaiseOnParentPackageInstalledEvent()
		{
			packageManagementEvents.OnParentPackageInstalled(new FakePackage("Test"));
		}
		
		void RaiseOnParentPackageUninstalledEvent()
		{
			packageManagementEvents.OnParentPackageUninstalled(new FakePackage("Test"));
		}

		[Test]
		public void OnParentPackageInstalled_EventFires_ProjectBrowserIsRefreshed()
		{
			CreateProjectBrowserRefresher();
			RaiseOnParentPackageInstalledEvent();
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
		
		[Test]
		public void OnParentPackageUninstalled_EventFires_ProjectBrowserIsRefreshed()
		{
			CreateProjectBrowserRefresher();
			RaiseOnParentPackageUninstalledEvent();
			
			Assert.IsTrue(fakeProjectService.IsRefreshProjectBrowserCalled);
		}
	}
}
