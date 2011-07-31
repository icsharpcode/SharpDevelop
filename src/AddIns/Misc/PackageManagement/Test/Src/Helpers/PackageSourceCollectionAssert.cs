// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;
using NUnit.Framework;

namespace PackageManagement.Tests.Helpers
{
	public class PackageSourceCollectionAssert
	{
		public static void AreEqual(IEnumerable<PackageSource> expectedSources, IEnumerable<PackageSourceViewModel> actualViewModels)
		{
			List<string> expectedSourcesAsList = ConvertToStrings(expectedSources);
			List<string> actualSources = ConvertToStrings(actualViewModels);
			
			CollectionAssert.AreEqual(expectedSourcesAsList, actualSources);
		}
		
		public static void AreEqual(IEnumerable<PackageSource> expectedSources, IEnumerable<PackageSource> actualSources)
		{
			List<string> expectedSourcesAsList = ConvertToStrings(expectedSources);
			List<string> actualSourcesAsList = ConvertToStrings(actualSources);
			
			CollectionAssert.AreEqual(expectedSourcesAsList, actualSourcesAsList);			
		}
		
		static List<string> ConvertToStrings(IEnumerable<PackageSource> sources)
		{
			List<string> convertedSources = new List<string>();
			foreach (PackageSource source in sources) {
				convertedSources.Add(ConvertToString(source));
			}
			return convertedSources;
		}
		
		static string ConvertToString(PackageSource source)
		{
			if (source != null) {
				return String.Format("[PackageSource] Name='{0}', Source='{1}'",
					source.Name,
					source.Source);
			}
			return "[PackageSource] == Null";
		}
		
		static List<string> ConvertToStrings(IEnumerable<PackageSourceViewModel> viewModels)
		{
			List<string> convertedSources = new List<string>();
			foreach (PackageSourceViewModel viewModel in viewModels) {
				PackageSource source = viewModel.GetPackageSource();
				convertedSources.Add(ConvertToString(source));
			}
			return convertedSources;
		}
	}
}
