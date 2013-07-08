// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class IPackageExtensions
	{
		/// <summary>
		/// Returns description if summary is missing.
		/// </summary>
		public static string SummaryOrDescription(this IPackage package)
		{
			if (String.IsNullOrEmpty(package.Summary))
				return package.Description;
			return package.Summary;
		}
		
		/// <summary>
		/// Returns package Id if it has no title.
		/// </summary>
		public static string GetName(this IPackage package)
		{
			if (String.IsNullOrEmpty(package.Title))
				return package.Id;
			return package.Title;
		}
	}
}
