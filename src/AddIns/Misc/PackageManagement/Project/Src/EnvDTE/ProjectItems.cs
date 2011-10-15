// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItems : MarshalByRefObject, IEnumerable<ProjectItem>
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
		
		void ThrowExceptionIfFileExists(string filePath)
		{
			if (fileService.FileExists(filePath)) {
				throw new FileExistsException(filePath);
			}
		}
		
		void CopyFileIntoProject(string oldFileName, string fileName)
		{
			string newFileName = GetFileNameInProject(fileName);
			ThrowExceptionIfFileExists(newFileName);
			fileService.CopyFile(oldFileName, newFileName);
		}
		
		string GetFileNameInProject(string fileName)
		{
			return Path.Combine(project.MSBuildProject.Directory, fileName);
		}
		
		public virtual IEnumerator<ProjectItem> GetEnumerator()
		{
			var items = new ProjectItemsInsideProject(project);
			return items.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public ProjectItem Item(string name)
		{
			foreach (ProjectItem item in this) {
				if (item.IsMatchByName(name)) {
					return item;
				}
			}
			return null;
		}
	}
}
