// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.AddInManager2.Model;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	public class LicenseAcceptanceViewModel : Model<LicenseAcceptanceViewModel>
	{
		IList<IPackage> packages;
		
		public LicenseAcceptanceViewModel(IEnumerable<IPackage> packages)
		{
			this.packages = packages.ToList();
		}
		
		public IEnumerable<IPackage> Packages
		{
			get
			{
				return packages;
			}
		}
		
		public bool HasOnePackage
		{
			get
			{
				return packages.Count == 1;
			}
		}
		
		public bool HasMultiplePackages
		{
			get
			{
				return packages.Count > 1;
			}
		}
	}
}
