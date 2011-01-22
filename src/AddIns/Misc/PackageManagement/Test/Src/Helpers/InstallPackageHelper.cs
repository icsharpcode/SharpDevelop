// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class InstallPackageHelper
	{
		PackageManagementService packageManagementService;
		
		public FakePackage TestPackage = new FakePackage() {
			Id = "Test"
		};
		
		public FakePackageRepository PackageRepository = new FakePackageRepository();
		
		public InstallPackageHelper(PackageManagementService packageManagementService)
		{
			this.packageManagementService = packageManagementService;
		}
		
		public void InstallTestPackage()
		{
			packageManagementService.InstallPackage(PackageRepository, TestPackage);
		}
	}
}
