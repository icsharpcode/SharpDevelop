// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageManagementProject : FakePackageManagementProject
	{
		public Exception ExceptionToThrowWhenCreateInstallPackageActionCalled { get; set; }
		public Exception ExceptionToThrowWhenCreateUninstallPackageActionCalled { get; set; }
		public Exception ExceptionToThrowWhenGetInstallPackageOperationsCalled { get; set; }

		public override InstallPackageAction CreateInstallPackageAction()
		{
			throw ExceptionToThrowWhenCreateInstallPackageActionCalled;
		}
		
		public override UninstallPackageAction CreateUninstallPackageAction()
		{
			throw ExceptionToThrowWhenCreateUninstallPackageActionCalled;
		}
		
		public override IEnumerable<PackageOperation> GetInstallPackageOperations(IPackage package, bool ignoreDependencies, bool allowPrereleaseVersions)
		{
			if (ExceptionToThrowWhenGetInstallPackageOperationsCalled != null) {
				throw ExceptionToThrowWhenGetInstallPackageOperationsCalled;
			}
			return base.GetInstallPackageOperations(package, ignoreDependencies, allowPrereleaseVersions);
		}
	}
}
