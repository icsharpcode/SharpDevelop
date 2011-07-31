// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementProjectFactory : IPackageManagementProjectFactory
	{
		SharpDevelopPackageManagerFactory factory = new SharpDevelopPackageManagerFactory();
		IPackageManagementEvents packageManagementEvents;
		
		public PackageManagementProjectFactory(IPackageManagementEvents packageManagementEvents)
		{
			this.packageManagementEvents = packageManagementEvents;
		}
		
		public IPackageManagementProject CreateProject(
			IPackageRepository sourceRepository,
			MSBuildBasedProject project)
		{
			return new PackageManagementProject(sourceRepository, project, packageManagementEvents, factory);
		}
	}
}
