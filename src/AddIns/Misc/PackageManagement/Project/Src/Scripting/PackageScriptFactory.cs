// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageScriptFactory : IPackageScriptFactory
	{
		public IPackageScript CreatePackageInitializeScript(IPackage package, string packageInstallDirectory)
		{
			var scriptFileName = new PackageInitializeScriptFileName(packageInstallDirectory);
			return new PackageInitializeScript(package, scriptFileName);
		}
		
		public IPackageScript CreatePackageUninstallScript(IPackage package, string packageInstallDirectory)
		{
			var scriptFileName = new PackageUninstallScriptFileName(packageInstallDirectory);
			return new PackageUninstallScript(package, scriptFileName);
		}
		
		public IPackageScript CreatePackageInstallScript(IPackage package, string packageInstallDirectory)
		{
			var scriptFileName = new PackageInstallScriptFileName(packageInstallDirectory);
			return new PackageInstallScript(package, scriptFileName);
		}
	}
}
