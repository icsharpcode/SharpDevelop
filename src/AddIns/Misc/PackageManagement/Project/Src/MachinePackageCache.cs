// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class MachinePackageCache : IMachinePackageCache
	{
		public IQueryable<IPackage> GetPackages()
		{
			return MachineCache.Default.GetPackages();
		}
		
		public void Clear()
		{
			MachineCache.Default.Clear();
		}
		
		public string Source {
			get { return MachineCache.Default.Source; }
		}
	}
}
