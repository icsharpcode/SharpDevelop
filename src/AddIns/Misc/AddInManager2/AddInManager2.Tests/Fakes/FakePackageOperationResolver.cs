// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakePackageOperationResolver : IPackageOperationResolver
	{
		public FakePackageOperationResolver()
		{
		}
		
		public System.Collections.Generic.IEnumerable<PackageOperation> ResolveOperations(IPackage package)
		{
			if (ResolveOperationsCallback != null)
			{
				return ResolveOperationsCallback(package);
			}
			else
			{
				return null;
			}
		}
		
		public Func<IPackage, IEnumerable<PackageOperation>> ResolveOperationsCallback
		{
			get;
			set;
		}
	}
}
