// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	public class PackageRepository : Model<PackageRepository>
	{
//		RegisteredPackageSource packageSource;
		
		public PackageRepository()
		{
		}
		
		public PackageRepository(PackageSource packageSource)
		{
//			this.packageSource = new RegisteredPackageSource(packageSource);
			Name = packageSource.Name;
			SourceUrl = packageSource.Source;
		}
		
		public string Name
		{
//			get
//			{
//				return packageSource.Name;
				// TODO
//				return null;
//			}
//			set
//			{
//				packageSource.Name = value;
//			}
			get;
			set;
		}
		
		public string SourceUrl
		{
//			get
//			{
//				return packageSource.Source;
				// TODO
//				return null;
//			}
//			set
//			{
//				packageSource.Source = value;
//			}
			get;
			set;
		}
		
		public PackageSource ToPackageSource()
		{
			return new PackageSource(SourceUrl, Name);
		}
	}
}
