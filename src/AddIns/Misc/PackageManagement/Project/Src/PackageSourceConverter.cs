// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class PackageSourceConverter
	{
		public static IEnumerable<PackageSource> ConvertFromRegisteredPackageSources(IEnumerable<RegisteredPackageSource> registeredPackageSources)
		{
			foreach (RegisteredPackageSource registeredPackageSource in registeredPackageSources) {
				yield return registeredPackageSource.ToPackageSource();
			}
		}
		
		public static IEnumerable<RegisteredPackageSource> ConvertToRegisteredPackageSources(IEnumerable<PackageSource> packageSources)
		{
			foreach (PackageSource packageSource in packageSources) {
				yield return new RegisteredPackageSource(packageSource);
			}
		}
		
		public static IList<RegisteredPackageSource> ConvertToRegisteredPackageSourcesList(IEnumerable<PackageSource> packageSources)
		{
			IEnumerable<RegisteredPackageSource> convertedPackageSources = ConvertToRegisteredPackageSources(packageSources);
			return new List<RegisteredPackageSource>(convertedPackageSources);
		}
	}
}
