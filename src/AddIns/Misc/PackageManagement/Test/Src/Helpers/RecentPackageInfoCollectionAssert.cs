// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public static class RecentPackageInfoCollectionAssert
	{
		public static void AreEqual(IEnumerable<RecentPackageInfo> expectedPackages, IEnumerable<RecentPackageInfo> actualPackages)
		{
			var expectedPackagesAsList = ConvertToStringList(expectedPackages);
			var actualPackagesAsList = ConvertToStringList(actualPackages);
			
			CollectionAssert.AreEqual(expectedPackagesAsList, actualPackagesAsList);		
		}
		
		static List<string> ConvertToStringList(IEnumerable<RecentPackageInfo> expectedPackages)
		{
			List<string> items = new List<string>();
			foreach (RecentPackageInfo recentPackage in expectedPackages) {
				items.Add(recentPackage.ToString());
			}
			return items;
		}
	}
}
