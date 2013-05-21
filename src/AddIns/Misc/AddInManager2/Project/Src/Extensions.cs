// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.AddInManager2
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> DistinctLast<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
		{
			T previousItem = default(T);
			
			foreach (T currentItem in source)
			{
				if (previousItem != null)
				{
					if (!comparer.Equals(previousItem, currentItem))
					{
						yield return previousItem;
					}
				}
				previousItem = currentItem;
			}
			
			if (previousItem != null)
			{
				yield return previousItem;
			}
		}
	}
	
	public static class SemanticVersionExtensions
	{
		public static Version ToVersion(this SemanticVersion semanticVersion)
		{
			string versionString = semanticVersion.ToString();
			if (!String.IsNullOrEmpty(semanticVersion.SpecialVersion))
			{
				// Remove special version from string (-1 for the "-" added before the version)
				versionString = versionString.Substring(0, versionString.Length - semanticVersion.SpecialVersion.Length - 1);
			}
			return new Version(versionString);
		}
	}
}
