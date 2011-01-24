// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ICSharpCode.Core;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredPackageSources : ObservableCollection<PackageSource>
	{
		public static readonly string DefaultPackageSourceUrl = "http://go.microsoft.com/fwlink/?LinkID=206669";
		public static readonly string DefaultPackageSourceName = "NuGet Official Package Source";
		
		public static readonly PackageSource DefaultPackageSource = 
			new PackageSource(DefaultPackageSourceUrl, DefaultPackageSourceName);
		
		public RegisteredPackageSources(IEnumerable<PackageSource> packageSources)
		{
			AddPackageSources(packageSources);
			AddDefaultPackageSourceIfNoRegisteredPackageSources();
		}

		void AddPackageSources(IEnumerable<PackageSource> packageSources)
		{
			foreach (PackageSource source in packageSources) {
				Add(source);
			}
		}
		
		void AddDefaultPackageSourceIfNoRegisteredPackageSources()
		{
			if (HasNoRegisteredPackageSources) {
				Add(DefaultPackageSource);
			}
		}
		
		bool HasNoRegisteredPackageSources {
			get { return Count == 0; }
		}
		
		public bool HasMultiplePackageSources {
			get { return Count > 1; }
		}
	}
}
