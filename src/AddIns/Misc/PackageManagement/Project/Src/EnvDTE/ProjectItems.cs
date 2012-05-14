// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItems : MarshalByRefObject, IEnumerable
	{
		Project project;
		IPackageManagementFileService fileService;
		
		public ProjectItems(Project project, IPackageManagementFileService fileService)
		{
			this.project = project;
			this.fileService = fileService;
		}
		
		public ProjectItems()
		{
		}
		
		public virtual object Parent {
			get { throw new NotImplementedException(); }
		}
		
		public virtual void AddFromFileCopy(string filePath)
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
		
		public virtual IEnumerator GetEnumerator()
		{
			var items = new ProjectItemsInsideProject(project);
			return items.GetEnumerator();
		}
		
		internal virtual ProjectItem Item(string name)
		{
			foreach (ProjectItem item in this) {
				if (item.IsMatchByName(name)) {
					return item;
				}
			}
			return null;
		}
		
		internal virtual ProjectItem Item(int index)
		{
			throw new NotImplementedException();
		}
		
		public virtual ProjectItem Item(object index)
		{
//			if (index is int) {
//				return Item((int)index);
//			}
//			return null;
			return Item(index as string);
		}
		
		public virtual ProjectItem AddFromDirectory(string directory)
		{
			throw new NotImplementedException();
		}
		
		public virtual ProjectItem AddFromFile(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public virtual int Count {
			get { return new ProjectItemsInsideProject(project).Count; }
		}
	}
}
