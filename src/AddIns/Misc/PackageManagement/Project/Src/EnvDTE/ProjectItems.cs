// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItems
	{
		Project project;
		IPackageManagementFileService fileService;
		
		public ProjectItems(Project project, IPackageManagementFileService fileService)
		{
			this.project = project;
			this.fileService = fileService;
		}
		
		public void AddFromFileCopy(string filePath)
		{
			string include = Path.GetFileName(filePath);
			CopyFileIntoProject(filePath, include);
			project.AddFile(include);
			project.Save();
		}
		
		void CopyFileIntoProject(string oldFileName, string fileName)
		{
			string newFileName = GetFileNameInProject(fileName);
			fileService.CopyFile(oldFileName, newFileName);
		}
		
		string GetFileNameInProject(string fileName)
		{
			return Path.Combine(project.MSBuildProject.Directory, fileName);
		}
	}
}
