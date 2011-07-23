// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class UpdatePackageActionsFactory : IUpdatePackageActionsFactory
	{
		public IUpdatePackageActions CreateUpdateAllPackagesInProject(IPackageManagementProject project)
		{
			return new UpdateAllPackagesInProject(project);
		}
	}
}
