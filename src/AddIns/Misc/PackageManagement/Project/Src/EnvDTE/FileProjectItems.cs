// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpDevelop.Gui;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	/// <summary>
	/// A file can have child project items if it has files that depend upon it.
	/// For example, winform designer files (MainForm.Designer.cs)
	/// </summary>
	public class FileProjectItems : ProjectItems
	{
		ProjectItem projectItem;
		IPackageManagementFileService fileService;
		
		public FileProjectItems(ProjectItem projectItem)
			: this(projectItem, new PackageManagementFileService())
		{
		}
		
		public FileProjectItems(ProjectItem projectItem, IPackageManagementFileService fileService)
			: base((Project)projectItem.ContainingProject, projectItem, fileService)
		{
			this.projectItem = projectItem;
			this.fileService = fileService;
		}
		
		protected override IEnumerable<global::EnvDTE.ProjectItem> GetProjectItems()
		{
			return GetChildDependentProjectItems().ToList();
		}
		
		IEnumerable<ProjectItem> GetChildDependentProjectItems()
		{
			foreach (SD.FileProjectItem fileProjectItem in GetFileProjectItems()) {
				if (fileProjectItem.IsDependentUpon(projectItem.MSBuildProjectItem)) {
					yield return new ProjectItem(Project, fileProjectItem);
				}
			}
		}
		
		IEnumerable<SD.FileProjectItem> GetFileProjectItems()
		{
			return Project
				.MSBuildProject
				.Items
				.Where(item => item is SD.FileProjectItem)
				.Select(item => (SD.FileProjectItem)item);
		}
		
		protected override ProjectItem AddFileProjectItemToProject(string fileName)
		{
			return AddFileProjectItemWithDependent(fileName);
		}
		
		ProjectItem AddFileProjectItemWithDependent(string fileName)
		{
			return Project.AddFileProjectItemWithDependentUsingFullPath(fileName, projectItem.Name);
		}
		
		public override string Kind {
			get { return global::EnvDTE.Constants.vsProjectItemKindPhysicalFile; }
		}
	}
}
