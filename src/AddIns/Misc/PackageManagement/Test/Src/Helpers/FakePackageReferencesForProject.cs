// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageReferencesForProject : IPackageReferencesForProject
	{
		public bool IsInstallPackagesCalled;
		
		public virtual void InstallPackages()
		{
			IsInstallPackagesCalled = true;
		}
		
		public bool IsRemovePackageReferencesCalled;
		
		public void RemovePackageReferences()
		{
			IsRemovePackageReferencesCalled = true;
		}
	}
}
