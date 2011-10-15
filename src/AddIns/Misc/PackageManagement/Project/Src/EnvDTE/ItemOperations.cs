// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ItemOperations : MarshalByRefObject
	{
		IPackageManagementFileService fileService;
		
		public ItemOperations(IPackageManagementFileService fileService)
		{
			this.fileService = fileService;
		}
		
		public void OpenFile(string fileName)
		{
			fileService.OpenFile(fileName);
		}
		
		public void Navigate(string url)
		{
			fileService.OpenFile(url);
		}
	}
}
