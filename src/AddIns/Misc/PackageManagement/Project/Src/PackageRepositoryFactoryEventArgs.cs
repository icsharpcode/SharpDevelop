// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageRepositoryFactoryEventArgs : EventArgs
	{
		public PackageRepositoryFactoryEventArgs(IPackageRepository repository)
		{
			this.Repository = repository;
		}
		
		public IPackageRepository Repository { get; private set; }
	}
}
