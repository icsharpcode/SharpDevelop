// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimePackageManagementOptionsViewModel : PackageManagementOptionsViewModel
	{
		public DesignTimePackageManagementOptionsViewModel()
			: this(new PackageManagementOptions(new Properties()))
		{
		}
		
		public DesignTimePackageManagementOptionsViewModel(PackageManagementOptions options)
			: base(options)
		{
			options.PackageSources.Add(new PackageSource("Source2", "http://sharpdevelop.codeplex.com"));
			options.PackageSources.Add(new PackageSource("Source3", "http://sharpdevelop.codeplex.com"));
			options.PackageSources.Add(new PackageSource("Source4", "http://sharpdevelop.codeplex.com"));
			options.PackageSources.Add(new PackageSource("Source5", "http://sharpdevelop.codeplex.com"));
			Load();
		}
	}
}
