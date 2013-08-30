// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IUpdatePackagesAction : IPackageAction, IUpdatePackageSettings
	{
		IEnumerable<PackageOperation> Operations { get; }
		ILogger Logger { get; set; }
		
		void AddPackages(IEnumerable<IPackageFromRepository> packages);
		void AddOperations(IEnumerable<PackageOperation> operations);
	}
}
