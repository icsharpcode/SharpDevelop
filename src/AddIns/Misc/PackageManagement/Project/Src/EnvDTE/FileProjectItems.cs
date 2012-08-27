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
		
		public FileProjectItems(ProjectItem projectItem)
			: base(projectItem.ContainingProject, projectItem, new PackageManagementFileService())
		{
			this.projectItem = projectItem;
		}
		
		protected override IEnumerable<ProjectItem> GetProjectItems()
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
	}
}
