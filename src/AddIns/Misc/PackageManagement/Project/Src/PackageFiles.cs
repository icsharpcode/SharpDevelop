// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageFiles
	{
		IEnumerable<IPackageFile> files;
		
		public PackageFiles(IPackage package)
			: this(package.GetFiles())
		{
		}
		
		public PackageFiles(IEnumerable<IPackageFile> files)
		{
			this.files = files;
		}
		
		public bool HasAnyPackageScripts()
		{
			foreach (string fileName in GetFileNames()) {
				if (IsPackageScriptFile(fileName)) {
					return true;
				}
			}
			return false;
		}
		
		IEnumerable<string> GetFileNames()
		{
			foreach (IPackageFile file in files) {
				string fileName = Path.GetFileName(file.Path);
				yield return fileName;
			}
		}
		
		public bool HasUninstallPackageScript()
		{
			foreach (string fileName in GetFileNames()) {
				if (IsPackageUninstallScriptFile(fileName)) {
					return true;
				}
			}
			return false;
		}
		
		bool IsPackageScriptFile(string fileName)
		{
			return
				IsPackageInitializationScriptFile(fileName) ||
				IsPackageInstallScriptFile(fileName) ||
				IsPackageUninstallScriptFile(fileName);
		}
		
		bool IsPackageInitializationScriptFile(string fileName)
		{
			return IsCaseInsensitiveMatch(fileName, "init.ps1");
		}
		
		bool IsPackageInstallScriptFile(string fileName)
		{
			return IsCaseInsensitiveMatch(fileName, "install.ps1");
		}
		
		bool IsPackageUninstallScriptFile(string fileName)
		{
			return IsCaseInsensitiveMatch(fileName, "uninstall.ps1");
		}
		
		bool IsCaseInsensitiveMatch(string a, string b)
		{
			return String.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
