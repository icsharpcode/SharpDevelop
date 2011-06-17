// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace PackageManagement.Tests.Helpers
{
	public class ExceptionThrowingPackageReferencesForProject : FakePackageReferencesForProject
	{
		public Exception ExceptionToThrowOnInstall;
		
		public override void InstallPackages()
		{
			throw ExceptionToThrowOnInstall;
		}
	}
}
