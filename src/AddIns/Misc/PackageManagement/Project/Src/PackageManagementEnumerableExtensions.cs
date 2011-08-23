// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class PackageManagementEnumerableExtensions
	{
		public static IEnumerable<TSource> DistinctLast<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
		{
			TSource previousItem = default(TSource);
			
			foreach (TSource currentItem in source) {
				if (previousItem != null) {
					if (!comparer.Equals(previousItem, currentItem)) {
						yield return previousItem;
					}
				}
				previousItem = currentItem;
			}
			
			if (previousItem != null) {
				yield return previousItem;
			}
		}
	}
}
