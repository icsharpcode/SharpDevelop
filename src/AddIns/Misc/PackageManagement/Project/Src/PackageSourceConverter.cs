// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class PackageSourceConverter
	{
		public static IEnumerable<PackageSource> ConvertFromKeyValuePairs(IEnumerable<KeyValuePair<string, string>> packageSources)
		{
			if (HasAny(packageSources)) {
				foreach (KeyValuePair<string, string> packageSource in packageSources) {
					yield return CreatePackageSourceFromKeyValuePair(packageSource);
				}
			}
		}
		
		static bool HasAny(IEnumerable<KeyValuePair<string, string>> packageSources)
		{
			if (packageSources != null) {
				return packageSources.Any();
			}
			return false;
		}
		
		static PackageSource CreatePackageSourceFromKeyValuePair(KeyValuePair<string, string> savedPackageSource)
		{
			string source = savedPackageSource.Value;
			string name = savedPackageSource.Key;
			return new PackageSource(source, name);
		}
		
		public static PackageSource ConvertFromFirstKeyValuePair(IEnumerable<KeyValuePair<string, string>> packageSources)
		{
			if (HasAny(packageSources)) {
				return CreatePackageSourceFromKeyValuePair(packageSources.First());
			}
			return null;
		}
		
		public static IList<KeyValuePair<string, string>> ConvertToKeyValuePairList(IEnumerable<PackageSource> packageSources)
		{
			var convertedPackageSources = new List<KeyValuePair<string, string>>();
			foreach (PackageSource source in packageSources) {
				convertedPackageSources.Add(ConvertToKeyValuePair(source));
			}
			return convertedPackageSources;
		}
		
		public static KeyValuePair<string, string> ConvertToKeyValuePair(PackageSource source)
		{
			return new KeyValuePair<string, string>(source.Name, source.Source);
		}
	}
}
