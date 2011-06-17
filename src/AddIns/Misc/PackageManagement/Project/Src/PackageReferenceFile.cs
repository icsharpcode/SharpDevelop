// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement
{
	public class PackageReferenceFile : IPackageReferenceFile
	{
		NuGet.PackageReferenceFile file;
		
		public PackageReferenceFile(string fileName)
		{
			file = new NuGet.PackageReferenceFile(fileName);
		}
		
		public IEnumerable<NuGet.PackageReference> GetPackageReferences()
		{
			return file.GetPackageReferences();
		}
		
		public void DeleteEntry(string id, Version version)
		{
			file.DeleteEntry(id, version);
		}
	}
}
