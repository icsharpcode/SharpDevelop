// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItemRelationship
	{
		public ProjectItemRelationship(ProjectItem parentProjectItem, SD.ProjectItem msbuildProjectItem)
		{
			this.ParentProjectItem = parentProjectItem;
			this.MSBuildProjectItem = msbuildProjectItem;
			this.Project = (Project)parentProjectItem.ContainingProject;
			GetRelationship();
		}
		
		public ProjectItem ParentProjectItem { get; private set; }
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
			return ParentProjectItem.IsChildItem(MSBuildProjectItem);
		}
		
		ProjectItem CreateProjectItem()
		{
			return new ProjectItem(Project, MSBuildProjectItem as FileProjectItem);
		}
		
		bool IsInChildDirectory()
		{
			return MSBuildProjectItemDirectory.StartsWith(ParentProjectItem.GetIncludePath());
		}
		
		ProjectItem CreateDirectoryItem()
		{
			string relativePath = GetPathOneDirectoryBelowParentProjectItem();
			return new DirectoryProjectItem(Project, relativePath);
		}
		
		string GetPathOneDirectoryBelowParentProjectItem()
		{
			string[] parentDirectories = ParentProjectItem.GetIncludePath().Split('\\');
			string[] directories = MSBuildProjectItemDirectory.Split('\\');
			return String.Join(@"\", directories.Take(parentDirectories.Length + 1).ToArray());
		}
	}
}
