// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public static class IPackageFromRepositoryExtensions
	{
		public static IDisposable StartInstallOperation(this IPackageFromRepository package)
		{
			return package.Repository.StartInstallOperation(package.Id);
		}
		
		public static IDisposable StartUpdateOperation(this IPackageFromRepository package)
		{
			return package.Repository.StartUpdateOperation(package.Id);
		}
	}
}
