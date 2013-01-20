// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class LicenseAcceptanceViewModel : ViewModelBase<LicenseAcceptanceViewModel>
	{
		IList<PackageLicenseViewModel> packages;
		
		public LicenseAcceptanceViewModel(IEnumerable<IPackage> packages)
		{
			this.packages = packages
				.Select(p => new PackageLicenseViewModel(p))
				.ToList();
		}
		
		public IEnumerable<PackageLicenseViewModel> Packages {
			get { return packages; }
		}
		
		public bool HasOnePackage {
			get { return packages.Count == 1; }
		}
		
		public bool HasMultiplePackages {
			get { return packages.Count > 1; }
		}
	}
}
