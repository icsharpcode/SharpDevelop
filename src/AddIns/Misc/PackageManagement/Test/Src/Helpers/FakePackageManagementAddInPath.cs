// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageManagementAddInPath : IPackageManagementAddInPath
	{
		public string CmdletsAssemblyFileName { get; set; }
		
		public List<string> PowerShellFormattingFileNames = new List<string>();
		
		public IEnumerable<string> GetPowerShellFormattingFileNames()
		{
			return PowerShellFormattingFileNames;
		}
	}
}
