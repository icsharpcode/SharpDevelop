// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageLicenseViewModel : ViewModelBase<PackageLicenseViewModel>
	{
		IPackage package;
		
		public PackageLicenseViewModel(IPackage package)
		{
			this.package = package;
		}
		
		public string Id {
			get { return package.Id; }
		}
		
		public string Summary {
			get { return package.SummaryOrDescription(); }
		}
		
		public Uri LicenseUrl {
			get { return package.LicenseUrl; }
		}
	}
}
