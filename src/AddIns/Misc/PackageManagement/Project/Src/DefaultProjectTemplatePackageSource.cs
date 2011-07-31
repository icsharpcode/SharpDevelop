// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class DefaultProjectTemplatePackageSource
	{
		PackageSource packageSource;
		
		public DefaultProjectTemplatePackageSource()
			: this(new PackageManagementPropertyService())
		{
		}
		
		public DefaultProjectTemplatePackageSource(IPropertyService propertyService)
		{
			CreatePackageSource(propertyService.DataDirectory);
		}
		
		void CreatePackageSource(string dataDirectory)
		{
			string packageDirectory = Path.Combine(dataDirectory, @"templates\packages");
			packageSource = new PackageSource(packageDirectory, "Default");
		}
		
		public PackageSource PackageSource {
			get { return packageSource; }
		}
	}
}
