// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItems : MarshalByRefObject, IEnumerable, global::EnvDTE.ProjectItems
	{
		IPackageManagementFileService fileService;
		object parent;
		
		public ProjectItems(Project project, object parent)
			: this(project, parent, project.FileService)
		{
		}
		
		ProjectItems(Project project, object parent, IPackageManagementFileService fileService)
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
			return new ProjectItemsInsideProject(Project, fileService);
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
