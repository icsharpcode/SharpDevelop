// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageManagementSolution : FakePackageManagementSolution
	{
		public Exception ExceptionToThrowWhenGetActiveProjectCalled { get; set; }
		public Exception ExceptionToThrowWhenCreateInstallPackageTaskCalled { get; set; }
		public Exception ExceptionToThrowWhenCreateUninstallPackageActionCalled { get; set; }

		public override InstallPackageAction CreateInstallPackageAction()
		{
			throw ExceptionToThrowWhenCreateInstallPackageTaskCalled;
		}
		
		public override UninstallPackageAction CreateUninstallPackageAction()
		{
			throw ExceptionToThrowWhenCreateUninstallPackageActionCalled;
		}
		
		public override IPackageManagementProject GetActiveProject()
		{
			if (ExceptionToThrowWhenGetActiveProjectCalled != null) {
				throw ExceptionToThrowWhenGetActiveProjectCalled;
			}
			return base.GetActiveProject();
		}
	}
}
