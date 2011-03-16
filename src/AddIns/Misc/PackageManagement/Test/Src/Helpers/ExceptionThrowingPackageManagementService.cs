// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageManagementService : FakePackageManagementService
	{
		public Exception ExeptionToThrowWhenActiveProjectManagerAccessed { get; set; }
		public Exception ExeptionToThrowWhenInstallPackageCalled { get; set; }
		public Exception ExeptionToThrowWhenUninstallPackageCalled { get; set; }

		public override IProjectManager ActiveProjectManager {
			get {
				if (ExeptionToThrowWhenActiveProjectManagerAccessed != null) {
					throw ExeptionToThrowWhenActiveProjectManagerAccessed;
				}
				return base.ActiveProjectManager;
			}
		}
		
		public override void InstallPackage(IPackageRepository repository, IPackage package, IEnumerable<PackageOperation> operations)
		{
			throw ExeptionToThrowWhenInstallPackageCalled;
		}
		
		public override void UninstallPackage(IPackageRepository repository, IPackage package)
		{
			throw ExeptionToThrowWhenUninstallPackageCalled;
		}
	}
}
