// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public interface IPackageScriptFactory
	{
		IPackageScript CreatePackageInitializeScript(string packageInstallDirectory);
		IPackageScript CreatePackageUninstallScript(string packageInstallDirectory);
		IPackageScript CreatePackageInstallScript(string packageInstallDirectory);
	}
}
