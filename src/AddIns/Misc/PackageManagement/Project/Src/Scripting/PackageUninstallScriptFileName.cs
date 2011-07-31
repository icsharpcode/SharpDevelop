// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageUninstallScriptFileName : PackageScriptFileName
	{
		public PackageUninstallScriptFileName(string packageInstallDirectory)
			: base(packageInstallDirectory)
		{
		}
		
		public PackageUninstallScriptFileName(IFileSystem fileSystem)
			: base(fileSystem)
		{
		}
		
		public override string Name {
			get { return "uninstall.ps1"; }
		}
	}
}
