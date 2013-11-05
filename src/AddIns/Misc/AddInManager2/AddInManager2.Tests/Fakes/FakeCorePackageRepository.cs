// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeCorePackageRepository : IPackageRepository
	{
		public FakeCorePackageRepository()
		{
		}
		
		public string Source
		{
			get;
			set;
		}
		
		public bool SupportsPrereleasePackages
		{
			get;
			set;
		}
		
		public IQueryable<IPackage> ReturnedPackages
		{
			get;
			set;
		}
		
		public System.Linq.IQueryable<IPackage> GetPackages()
		{
			return ReturnedPackages;
		}
		
		public void AddPackage(IPackage package)
		{
			if (AddPackageCallback != null)
			{
				AddPackageCallback(package);
			}
		}
		
		public Action<IPackage> AddPackageCallback
		{
			get;
			set;
		}
		
		public void RemovePackage(IPackage package)
		{
			if (RemovePackageCallback != null)
			{
				RemovePackageCallback(package);
			}
		}
		
		public Action<IPackage> RemovePackageCallback
		{
			get;
			set;
		}
	}
}
