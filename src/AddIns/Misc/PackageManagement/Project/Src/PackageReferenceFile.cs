// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageReferenceFile : IPackageReferenceFile
	{
		NuGet.PackageReferenceFile file;
		string fileName;
		
		public PackageReferenceFile(string fileName)
		{
			this.fileName = fileName;
			this.file = new NuGet.PackageReferenceFile(fileName);
		}
		
		public IEnumerable<NuGet.PackageReference> GetPackageReferences()
		{
			return file.GetPackageReferences();
		}
		
		public void DeleteEntry(string id, SemanticVersion version)
		{
			file.DeleteEntry(id, version);
		}
		
		public void Delete()
		{
			FileService.RemoveFile(fileName, false);
		}
	}
}
