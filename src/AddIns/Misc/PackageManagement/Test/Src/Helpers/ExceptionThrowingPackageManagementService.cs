// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageManagementService : FakePackageManagementService
	{
		public Exception ExeptionToThrowWhenActiveProjectManagerAccessed { get; set; }
		public Exception ExeptionToThrowWhenCreateInstallPackageTaskCalled { get; set; }
		public Exception ExeptionToThrowWhenCreateUninstallPackageActionCalled { get; set; }
		public Exception ExeptionToThrowWhenActiveRepositoryAccessed { get; set; }
		public Exception ExceptionToThrowWhenCreatePackageManagerForActiveProjectCalled { get; set; }

		public override IProjectManager ActiveProjectManager {
			get {
				if (ExeptionToThrowWhenActiveProjectManagerAccessed != null) {
					throw ExeptionToThrowWhenActiveProjectManagerAccessed;
				}
				return base.ActiveProjectManager;
			}
		}
		
		public override IPackageRepository ActivePackageRepository {
			get {
				if (ExeptionToThrowWhenActiveRepositoryAccessed != null) {
					throw ExeptionToThrowWhenActiveRepositoryAccessed;
				}
				return base.ActivePackageRepository;
			}
			set { base.ActivePackageRepository = value; }
		}
		
		public override InstallPackageAction CreateInstallPackageAction()
		{
			throw ExeptionToThrowWhenCreateInstallPackageTaskCalled;
		}
		
		public override UninstallPackageAction CreateUninstallPackageAction()
		{
			throw ExeptionToThrowWhenCreateUninstallPackageActionCalled;
		}
		
		public override ISharpDevelopPackageManager CreatePackageManagerForActiveProject()
		{
			if (ExceptionToThrowWhenCreatePackageManagerForActiveProjectCalled != null) {
				throw ExceptionToThrowWhenCreatePackageManagerForActiveProjectCalled;
			}
			return base.CreatePackageManagerForActiveProject();
		}
	}
}
