// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
