// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageSourceViewModel : ViewModelBase<PackageSourceViewModel>
	{
		RegisteredPackageSource packageSource;
		
		public PackageSourceViewModel(PackageSource packageSource)
		{
			this.packageSource = new RegisteredPackageSource(packageSource);
		}
		
		public PackageSource GetPackageSource()
		{
			return packageSource.ToPackageSource();
		}
		
		public string Name {
			get { return packageSource.Name; }
			set { packageSource.Name = value; }
		}
		
		public string SourceUrl {
			get { return packageSource.Source; }
			set { packageSource.Source = value; }
		}
		
		public bool IsEnabled {
			get { return packageSource.IsEnabled; }
			set { packageSource.IsEnabled = value; }
		}
	}
}
