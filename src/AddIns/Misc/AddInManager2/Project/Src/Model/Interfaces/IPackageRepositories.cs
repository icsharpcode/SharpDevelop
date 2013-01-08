// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public interface IPackageRepositories
	{
		IPackageRepository Registered
		{
			get;
		}
		
		IPackageRepository Active
		{
			get;
		}
		
		PackageSource ActiveSource
		{
			get;
			set;
		}
		IEnumerable<PackageSource> RegisteredPackageSources
		{
			get;
			set;
		}
	}
}
