// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageReferenceFile : IPackageReferenceFile
	{
		public List<PackageReference> FakePackageReferences = new List<PackageReference>();
		
		public void AddFakePackageReference(string packageId, string version)
		{
			var packageReference = new PackageReference(packageId, new SemanticVersion(version), null, null);
			FakePackageReferences.Add(packageReference);
		}
		
		public IEnumerable<PackageReference> GetPackageReferences()
		{
			return FakePackageReferences;
		}
		
		public List<PackageReference> EntriesDeleted = new List<PackageReference>();
		
		public void DeleteEntry(string id, SemanticVersion version)
		{
			var packageReference = new PackageReference(id, version, null, null);
			EntriesDeleted.Add(packageReference);
		}
		
		public bool IsDeleteCalled;
		
		public void Delete()
		{
			IsDeleteCalled = true;
		}
	}
}
