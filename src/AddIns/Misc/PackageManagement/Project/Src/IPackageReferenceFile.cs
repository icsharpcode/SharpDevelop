// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public interface IPackageReferenceFile
	{
		IEnumerable<PackageReference> GetPackageReferences();
		void DeleteEntry(string id, SemanticVersion version);
		void Delete();
	}
}
