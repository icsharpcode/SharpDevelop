// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageFilesForOperations
	{
		IEnumerable<PackageOperation> operations;
		
		public PackageFilesForOperations(IEnumerable<PackageOperation> operations)
		{
			this.operations = operations;
		}
		
		public bool HasAnyPackageScripts()
		{
			foreach (PackageFiles files in GetPackageFilesForEachOperation()) {
				if (files.HasAnyPackageScripts()) {
					return true;
				}
			}
			return false;
		}
		
		IEnumerable<PackageFiles> GetPackageFilesForEachOperation()
		{
			foreach (PackageOperation operation in operations) {
				yield return new PackageFiles(operation.Package);
			}
		}
	}
}
