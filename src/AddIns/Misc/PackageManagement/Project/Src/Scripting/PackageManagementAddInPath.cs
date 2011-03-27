// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageManagementAddInPath : IPackageManagementAddInPath
	{
		string cmdletsAssemblyFileName;
		
		public string CmdletsAssemblyFileName {
			get {
				if (cmdletsAssemblyFileName == null) {
					GetCmdletsAssemblyFileName();
				}
				return cmdletsAssemblyFileName;
			}
		}
		
		void GetCmdletsAssemblyFileName()
		{
			cmdletsAssemblyFileName = Path.Combine(AddInDirectory, "PackageManagement.Cmdlets.dll");
		}
		
		string AddInDirectory {
			get {
				string addinFilename = GetType().Assembly.Location;
				return Path.GetDirectoryName(addinFilename);
			}
		}
		
		public IEnumerable<string> GetPowerShellFormattingFileNames()
		{
			return Directory.GetFiles(PowerShellScriptsDirectory, "*.ps1xml");
		}
		
		string PowerShellScriptsDirectory {
			get { return Path.Combine(AddInDirectory, "Scripts"); }
		}
	}
}
