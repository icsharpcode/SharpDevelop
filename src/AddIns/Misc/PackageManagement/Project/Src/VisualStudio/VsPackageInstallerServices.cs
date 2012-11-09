// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;
using NuGet.VisualStudio;

namespace ICSharpCode.PackageManagement.VisualStudio
{
	public class VsPackageInstallerServices : IVsPackageInstallerServices
	{
		IPackageManagementSolution solution;
		
		public VsPackageInstallerServices()
			: this(PackageManagementServices.Solution)
		{
		}
		
		public VsPackageInstallerServices(IPackageManagementSolution solution)
		{
			this.solution = solution;
		}
		
		public IEnumerable<IVsPackageMetadata> GetInstalledPackages()
		{
			foreach (IPackage package in solution.GetPackages()) {
				string installPath = solution.GetInstallPath(package);
				yield return new VsPackageMetadata(package, installPath);
			}
		}
	}
}
