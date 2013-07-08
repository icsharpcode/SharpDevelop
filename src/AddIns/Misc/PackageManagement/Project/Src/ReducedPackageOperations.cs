// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class ReducedPackageOperations
	{
		IPackageOperationResolver resolver;
		IList<PackageOperation> operations;
		IEnumerable<IPackage> packages;
		
		public ReducedPackageOperations(IPackageOperationResolver resolver, IEnumerable<IPackage> packages)
		{
			this.resolver = resolver;
			this.packages = packages;
			this.operations = new List<PackageOperation>();
		}
		
		public IEnumerable<PackageOperation> Operations {
			get { return operations; }
		}
		
		public void Reduce()
		{
			foreach (IPackage package in packages) {
				if (!InstallOperationExists(package)) {
					operations.AddRange(resolver.ResolveOperations(package));
				}
			}
			
			operations = operations.Reduce();
		}
		
		bool InstallOperationExists(IPackage package)
		{
			var installOperation = new PackageOperation(package, PackageAction.Install);
			return operations.Any(operation => IsMatch(installOperation, operation));
		}
		
		bool IsMatch(PackageOperation x, PackageOperation y)
		{
			return (x.Package.Id == y.Package.Id) &&
				(x.Package.Version == y.Package.Version) &&
				(x.Action == y.Action);
		}
	}
}
