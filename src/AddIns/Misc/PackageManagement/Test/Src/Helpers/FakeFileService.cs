// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.SharpDevelop.Project;

namespace PackageManagement.Tests.Helpers
{
	public class FakeFileService : IPackageManagementFileService
	{
		public string PathPassedToRemoveFile;
		public string PathPassedToRemoveDirectory;
		
		MSBuildBasedProject project;
		
		public FakeFileService(MSBuildBasedProject project)
		{
			this.project = project;
		}
		
		public void RemoveFile(string path)
		{
			PathPassedToRemoveFile = path;
			
			RemoveFirstProjectItem();
		}
		
		public void RemoveDirectory(string path)
		{
			PathPassedToRemoveDirectory = path;
			
			RemoveFirstProjectItem();
		}
		
		void RemoveFirstProjectItem()
		{
			ProjectItem item = project.Items[0];
			ProjectService.RemoveProjectItem(project, item);
		}
	}
}
