// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class DesignTimeRegisteredPackageSourcesViewModel : RegisteredPackageSourcesViewModel
	{
		public DesignTimeRegisteredPackageSourcesViewModel()
			: this(new PackageManagementOptions(new Properties(), new FakeSettings()))
		{
		}
		
		public DesignTimeRegisteredPackageSourcesViewModel(PackageManagementOptions options)
			: base(options.PackageSources)
		{
			options.PackageSources.Add(new PackageSource("Source2", "http://sharpdevelop.codeplex.com"));
			options.PackageSources.Add(new PackageSource("Source3", "http://sharpdevelop.codeplex.com"));
			options.PackageSources.Add(new PackageSource("Source4", "http://sharpdevelop.codeplex.com"));
			options.PackageSources.Add(new PackageSource("Source5", "http://sharpdevelop.codeplex.com"));
			Load();
		}
	}
}
