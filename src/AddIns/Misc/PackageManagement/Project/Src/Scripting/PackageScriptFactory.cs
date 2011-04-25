// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageScriptFactory : IPackageScriptFactory
	{
		IPackageScriptSession session;
		
		public PackageScriptFactory(IPackageScriptSession session)
		{
			this.session = session;
		}
		
		public IPackageScript CreatePackageInitializeScript(string packageInstallDirectory)
		{
			var scriptFileName = new PackageInitializeScriptFileName(packageInstallDirectory);
			return new PackageInitializeScript(scriptFileName, session);
		}
		
		public IPackageScript CreatePackageUninstallScript(string packageInstallDirectory)
		{
			var scriptFileName = new PackageUninstallScriptFileName(packageInstallDirectory);
			return new PackageUninstallScript(scriptFileName, session);
		}
		
		public IPackageScript CreatePackageInstallScript(string packageInstallDirectory)
		{
			var scriptFileName = new PackageInstallScriptFileName(packageInstallDirectory);
			return new PackageInstallScript(scriptFileName, session);
		}
	}
}
