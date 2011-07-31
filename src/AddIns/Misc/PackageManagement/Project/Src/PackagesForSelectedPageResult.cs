// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackagesForSelectedPageResult
	{
		public PackagesForSelectedPageResult(IEnumerable<IPackage> packages, int totalPackages)
		{
			this.Packages = packages;
			this.TotalPackagesOnPage = packages.Count();
			this.TotalPackages = totalPackages;
		}
		
		public IEnumerable<IPackage> Packages { get; set; }
		public int TotalPackagesOnPage { get; set; }
		public int TotalPackages { get; set; }
	}
}
