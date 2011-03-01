// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class PackageCollectionAssert
	{
		public static void AreEqual(IEnumerable<IPackage> expectedPackages, IEnumerable<PackageViewModel> actualViewModels)
		{
			var expectedPackagesAsList = new List<IPackage>(expectedPackages);
			List<IPackage> actualPackages = ConvertToPackageList(actualViewModels);
			
			CollectionAssert.AreEqual(expectedPackagesAsList, actualPackages);
		}
		
		static List<IPackage> ConvertToPackageList(IEnumerable<PackageViewModel> viewModels)
		{
			var packages = new List<IPackage>();
			foreach (PackageViewModel viewModel in viewModels) {
				packages.Add(viewModel.GetPackage());
			}
			return packages;
		}
		
		public static void AreEqual(IEnumerable<IPackage> expectedPackages, IEnumerable<IPackage> actualPackages)
		{
			var expectedPackagesAsList = new List<IPackage>(expectedPackages);
			var actualPackagesAsList = new List<IPackage>(actualPackages);
			
			CollectionAssert.AreEqual(expectedPackagesAsList, actualPackagesAsList);		
		}
	}
}
