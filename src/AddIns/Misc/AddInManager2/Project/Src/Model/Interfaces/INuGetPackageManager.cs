// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public interface INuGetPackageManager
	{
		IPackageManager Packages
		{
			get;
		}
		string PackageOutputDirectory
		{
			get;
		}
		IPackageOperationResolver CreateInstallPackageOperationResolver(bool allowPrereleaseVersions);
		void ExecuteOperation(PackageOperation operation);
		string GetLocalPackageDirectory(IPackage package);
	}
}
