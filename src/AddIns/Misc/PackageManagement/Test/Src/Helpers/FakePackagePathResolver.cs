// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackagePathResolver : IPackagePathResolver
	{
		public string GetInstallPath(IPackage package)
		{
			throw new NotImplementedException();
		}
		
		public string GetPackageDirectory(IPackage package)
		{
			throw new NotImplementedException();
		}
		
		public string GetPackageDirectory(string packageId, SemanticVersion version)
		{
			throw new NotImplementedException();
		}
		
		public string GetPackageFileName(IPackage package)
		{
			throw new NotImplementedException();
		}
		
		public string GetPackageFileName(string packageId, SemanticVersion version)
		{
			throw new NotImplementedException();
		}
	}
}
