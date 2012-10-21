// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItems : MarshalByRefObject, IEnumerable, global::EnvDTE.ProjectItems
	{
		IPackageManagementFileService fileService;
		object parent;
		
		public ProjectItems(Project project, object parent, IPackageManagementFileService fileService)
		{
			this.Project = project;
			this.fileService = fileService;
			this.parent = parent;
		}
		
		public ProjectItems()
		{
		}
		
		protected Project Project { get; private set; }
		
		public virtual object Parent {
			get { return parent; }
		}
		
		public virtual void AddFromFileCopy(string filePath)
		{
			string fileAdded = filePath;
			if (IsFileInsideProjectFolder(filePath)) {
				ThrowExceptionIfFileDoesNotExist(filePath);
			} else {
				fileAdded = CopyFileIntoProject(filePath);
			}
			
			using (IProjectBrowserUpdater updater = Project.CreateProjectBrowserUpdater()) {
				AddFileProjectItemToProject(fileAdded);
				Project.Save();
			}
		}
		
		/// <summary>
		/// The file will be copied inside the folder for the parent containing 
		/// these project items.
		/// </summary>
		string GetIncludePathForFileCopy(string filePath)
		{
			string fileNameWithoutAnyPath = Path.GetFileName(filePath);
			if (Parent is Project) {
				return fileNameWithoutAnyPath;
			}
			var item = Parent as ProjectItem;
			return item.GetIncludePath(fileNameWithoutAnyPath);
		}
		
		bool IsFileInsideProjectFolder(string filePath)
		{
			return Project.IsFileFileInsideProjectFolder(filePath);
		}
		
		void ThrowExceptionIfFileDoesNotExist(string filePath)
		{
			if (!fileService.FileExists(filePath)) {
				throw new FileNotFoundException("Cannot find file", filePath);
			}
		}
		
		void ThrowExceptionIfFileExists(string filePath)
		{
			if (fileService.FileExists(filePath)) {
				throw new FileExistsException(filePath);
			}
		}
		
		string CopyFileIntoProject(string fileName)
		{
			string projectItemInclude = GetIncludePathForFileCopy(fileName);
			string newFileName = GetFileNameInProjectFromProjectItemInclude(projectItemInclude);
			ThrowExceptionIfFileExists(newFileName);
			fileService.CopyFile(fileName, newFileName);
			return newFileName;
		}
		
		string GetFileNameInProjectFromProjectItemInclude(string projectItemInclude)
		{
			return Path.Combine(Project.MSBuildProject.Directory, projectItemInclude);
		}
		
		public virtual IEnumerator GetEnumerator()
		{
			return GetProjectItems().GetEnumerator();
		}
		
		protected virtual IEnumerable<global::EnvDTE.ProjectItem> GetProjectItems()
		{
			return new ProjectItemsInsideProject(Project);
		}
		
		internal virtual ProjectItem Item(string name)
		{
			foreach (ProjectItem item in this) {
				if (item.IsMatchByName(name)) {
					return item;
				}
			}
			throw new ArgumentException("Unable to find item: " + name, "name");
		}
		
		internal virtual ProjectItem Item(int index)
		{
			return GetProjectItems()
				.Skip(index - 1)
				.First() as ProjectItem;
		}
		
		public virtual global::EnvDTE.ProjectItem Item(object index)
		{
			if (index is int) {
				return Item((int)index);
			}
			return Item(index as string);
		}
		
		public virtual global::EnvDTE.ProjectItem AddFromDirectory(string directory)
		{
			using (IProjectBrowserUpdater updater = Project.CreateProjectBrowserUpdater()) {
				ProjectItem directoryItem = Project.AddDirectoryProjectItemUsingFullPath(directory);
				Project.Save();
				return directoryItem;
			}
		}
		
		public virtual global::EnvDTE.ProjectItem AddFromFile(string fileName)
		{
			using (IProjectBrowserUpdater updater = Project.CreateProjectBrowserUpdater()) {
				ProjectItem projectItem = AddFileProjectItemToProject(fileName);
				Project.Save();
				fileService.ParseFile(fileName);
				return projectItem;
			}
		}
		
		/// <summary>
		/// Adds a file to the project with this ProjectItems as its parent.
		/// </summary>
		protected virtual ProjectItem AddFileProjectItemToProject(string fileName)
		{
			return Project.AddFileProjectItemUsingFullPath(fileName);
		}
		
		public virtual int Count {
			get { return GetProjectItems().Count(); }
		}
		
		public virtual string Kind {
			get { return global::EnvDTE.Constants.vsProjectItemKindPhysicalFolder; }
		}
	}
}
