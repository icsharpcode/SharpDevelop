// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredPackageSource
	{
		public string Source { get; set; }
		public string Name { get; set; }
		public bool IsEnabled { get; set; }
		
		public RegisteredPackageSource()
		{
		}
		
		public RegisteredPackageSource(PackageSource packageSource)
		{
			Source = packageSource.Source;
			Name = packageSource.Name;
			IsEnabled = packageSource.IsEnabled;
		}
		
		public PackageSource ToPackageSource()
		{
			return new PackageSource(Source, Name, IsEnabled);
		}
	}
}
