// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class SharpDevelopPackageRepositoryFactory : PackageRepositoryFactory, ISharpDevelopPackageRepositoryFactory
	{	
		public ISharedPackageRepository CreateSharedRepository(IPackagePathResolver pathResolver, IFileSystem fileSystem)
		{
			return new SharedPackageRepository(pathResolver, fileSystem);
		}
	}
}
