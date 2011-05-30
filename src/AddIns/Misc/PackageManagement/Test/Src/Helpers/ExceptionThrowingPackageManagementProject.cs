// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.Design;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageManagementProject : FakePackageManagementProject
	{
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
	}
}
