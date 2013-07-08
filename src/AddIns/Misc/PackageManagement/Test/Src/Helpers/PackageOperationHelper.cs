// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public static class PackageOperationHelper
	{
		public static PackageOperation CreateInstallOperationWithFile(string fileName)
		{
			var package = new FakePackage();
			package.AddFile(fileName);
			
			return new PackageOperation(package, PackageAction.Install);
		}
		
		public static List<PackageOperation> CreateListWithOneInstallOperationWithFile(string fileName)
		{
			PackageOperation operation = CreateInstallOperationWithFile(fileName);
			var operations = new List<PackageOperation>();
			operations.Add(operation);
			return operations;
		}
	}
}
