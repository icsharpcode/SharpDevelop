// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Scripting;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptFactory : IPackageScriptFactory
	{
		public string PackageInstallDirectoryPassed;
		public FakePackageScript FakePackageInitializeScript = new FakePackageScript();
		public FakePackageScript FakePackageInstallScript = new FakePackageScript();
		public FakePackageScript FakePackageUninstallScript = new FakePackageScript();
		
		public IPackageScript CreatePackageInitializeScript(string packageInstallDirectory)
		{
			PackageInstallDirectoryPassed = packageInstallDirectory;
			return FakePackageInitializeScript;
		}
		
		public IPackageScript CreatePackageUninstallScript(string packageInstallDirectory)
		{
			PackageInstallDirectoryPassed = packageInstallDirectory;
			return FakePackageUninstallScript;
		}
		
		public IPackageScript CreatePackageInstallScript(string packageInstallDirectory)
		{
			PackageInstallDirectoryPassed = packageInstallDirectory;
			return FakePackageInstallScript;
		}
	}
}
