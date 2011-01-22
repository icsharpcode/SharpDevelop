// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementFileService : IPackageManagementFileService
	{
		public void RemoveFile(string path)
		{
			FileService.RemoveFile(path, false);
		}
		
		public void RemoveDirectory(string path)
		{
			FileService.RemoveFile(path, true);
		}
	}
}
