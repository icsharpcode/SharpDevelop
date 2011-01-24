// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class OneRegisteredPackageSourceHelper
	{
		public RegisteredPackageSources RegisteredPackageSources;
		public PackageManagementOptions Options;
		public PackageSource PackageSource = new PackageSource("http://sharpdevelop.com", "Test Package Source");
		
		public OneRegisteredPackageSourceHelper()
		{
			CreateOneRegisteredPackageSource();
		}
		
		void CreateOneRegisteredPackageSource()
		{
			Properties properties = new Properties();
			Options = new PackageManagementOptions(properties);
			RegisteredPackageSources = Options.PackageSources;
			AddOnePackageSource();
		}
		
		public void AddOnePackageSource()
		{
			RegisteredPackageSources.Clear();
			RegisteredPackageSources.Add(PackageSource);
		}
		
		public void AddTwoPackageSources()
		{			
			AddOnePackageSource();
			RegisteredPackageSources.Add(new PackageSource("http://second.codeplex.com", "second"));
		}
	}
}
