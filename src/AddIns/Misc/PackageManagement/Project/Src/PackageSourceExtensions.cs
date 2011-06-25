// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public static class PackageSourceExtensions
	{
		public static bool IsAggregate(this PackageSource packageSource)
		{
			return packageSource.Source == RegisteredPackageSourceSettings.AggregatePackageSource.Source;
		}
	}
}
