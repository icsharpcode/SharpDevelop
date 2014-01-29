// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class IPackageRepositoryExtensions
	{
		public static IDisposable StartInstallOperation(this IPackageRepository repository, string mainPackageId = null, string mainPackageVersion = null)
		{
			return repository.StartOperation(RepositoryOperationNames.Install, mainPackageId, mainPackageVersion);
		}
		
		public static IDisposable StartUpdateOperation(this IPackageRepository repository, string mainPackageId = null, string mainPackageVersion = null)
		{
			return repository.StartOperation(RepositoryOperationNames.Update, mainPackageId, mainPackageVersion);
		}
	}
}
