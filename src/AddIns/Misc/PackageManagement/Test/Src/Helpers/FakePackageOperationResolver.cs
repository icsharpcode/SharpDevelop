// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageOperationResolver : IPackageOperationResolver
	{
		public List<PackageOperation> PackageOperations = new List<PackageOperation>();
		
		public virtual IEnumerable<PackageOperation> ResolveOperations(IPackage package)
		{
			return PackageOperations;
		}
	}
}
