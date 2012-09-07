// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.PackageManagement.Design;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeSharedPackageRepository : FakePackageRepository, ISharedPackageRepository
	{
		public string PathPassedToRegisterRepository;
		
		public List<string> PackageIdsReferences = new List<string>();
		public string PackageIdPassedToIsReferenced;
		public SemanticVersion VersionPassedToIsReferenced;
		
		public bool IsReferenced(string packageId, SemanticVersion version)
		{
			PackageIdPassedToIsReferenced = packageId;
			VersionPassedToIsReferenced = version;
			return PackageIdsReferences.Contains(packageId);
		}
		
		public void RegisterRepository(string path)
		{
			PathPassedToRegisterRepository = path;
		}
		
		public void UnregisterRepository(string path)
		{
			throw new NotImplementedException();
		}
		
		public void AddPackageReferenceEntry(string packageId, SemanticVersion version)
		{
			throw new NotImplementedException();
		}
	}
}
