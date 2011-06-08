// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItemRelationship
	{
		public ProjectItemRelationship(ProjectItem projectItem, SD.ProjectItem msbuildProjectItem)
		{
			this.ProjectItem = projectItem;
			this.MSBuildProjectItem = msbuildProjectItem;
			this.Project = projectItem.ContainingProject;
			GetRelationship();
		}
		
		public ProjectItem ProjectItem { get; private set; }
		public SD.ProjectItem MSBuildProjectItem { get; private set; }
		public Project Project { get; private set; }
		
		string MSBuildProjectItemDirectory;
		
		void GetRelationship()
		{
			GetMSBuildProjectItemDirectory();
		}
		
		void GetMSBuildProjectItemDirectory()
		{
			MSBuildProjectItemDirectory = Path.GetDirectoryName(MSBuildProjectItem.Include);
		}
		
		public ProjectItem GetChild()
		{
			if (IsChildItem()) {
				return CreateProjectItem();
			} else {
				if (IsInChildDirectory()) {
					return CreateDirectoryItem();
				}
			}
			return null;
		}
		
		bool IsChildItem()
		{
			return ProjectItem.IsChildItem(MSBuildProjectItem);
		}
		
		ProjectItem CreateProjectItem()
		{
			return new ProjectItem(Project, MSBuildProjectItem as FileProjectItem);
		}
		
		bool IsInChildDirectory()
		{
			return MSBuildProjectItemDirectory.StartsWith(ProjectItem.Name);
		}
		
		ProjectItem CreateDirectoryItem()
		{
			return new DirectoryProjectItem(Project, MSBuildProjectItemDirectory);
		}
	}
}
