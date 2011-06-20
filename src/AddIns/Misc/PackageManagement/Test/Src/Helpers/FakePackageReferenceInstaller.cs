// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageReferenceInstaller : IPackageReferenceInstaller
	{
		public IEnumerable<PackageReference> PackageReferencesPassedToInstallPackages;
		public MSBuildBasedProject ProjectPassedToInstallPackages;
		
		public void InstallPackages(IEnumerable<PackageReference> packageReferences, MSBuildBasedProject project)
		{
			PackageReferencesPassedToInstallPackages = packageReferences;
			ProjectPassedToInstallPackages = project;
		}
	}
}
