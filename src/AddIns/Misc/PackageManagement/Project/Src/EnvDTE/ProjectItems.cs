// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItems : MarshalByRefObject, IEnumerable
	{
		Project project;
		IPackageManagementFileService fileService;
		object parent;
		
		public ProjectItems(Project project, object parent, IPackageManagementFileService fileService)
		{
			this.project = project;
			this.fileService = fileService;
			this.parent = parent;
		}
		
		public ProjectItems()
		{
		}
		
		public virtual object Parent {
			get { return parent; }
		}
		
		public virtual void AddFromFileCopy(string filePath)
		{
			string include = Path.GetFileName(filePath);
			CopyFileIntoProject(filePath, include);
			project.AddFileUsingPathRelativeToProject(include);
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
			var items = new ProjectItemsInsideProject(project);
			return items.GetItem(index - 1);
		}
		
		public virtual ProjectItem Item(object index)
		{
			if (index is int) {
				return Item((int)index);
			}
			return Item(index as string);
		}
		
		public virtual ProjectItem AddFromDirectory(string directory)
		{
			throw new NotImplementedException();
		}
		
		public virtual ProjectItem AddFromFile(string fileName)
		{
			ProjectItem projectItem = project.AddFileUsingFullPath(fileName);
			project.Save();
			return projectItem;
		}
		
		public virtual int Count {
			get { return new ProjectItemsInsideProject(project).Count; }
		}
	}
}
