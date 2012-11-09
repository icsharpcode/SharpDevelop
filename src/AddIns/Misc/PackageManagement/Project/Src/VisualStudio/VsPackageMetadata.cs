// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;
using NuGet.VisualStudio;

namespace ICSharpCode.PackageManagement.VisualStudio
{
	public class VsPackageMetadata : IVsPackageMetadata
	{
		IPackage package;
		
		public VsPackageMetadata(IPackage package, string installPath)
		{
			this.package = package;
			this.InstallPath = installPath;
		}
		
		public string Id {
			get { return package.Id; }
		}
		
		public SemanticVersion Version {
			get { return package.Version; }
		}
		
		public string InstallPath { get; private set; }
	}
}
