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
	public class DirectoryProjectItem : ProjectItem
	{
		string relativePath;
		
		public DirectoryProjectItem(
			Project project,
			string relativePath)
			: this(project, CreateFileProjectItem(project, relativePath))
		{
			this.relativePath = relativePath;
		}
		
		static FileProjectItem CreateFileProjectItem(Project project, string relativePath)
		{
			return new FileProjectItem(project.MSBuildProject, ItemType.Folder, relativePath);
		}
		
		static string GetSubdirectoryName(string relativePath)
		{
			return relativePath.Split('\\').First();
		}
		
		public DirectoryProjectItem(Project project, FileProjectItem projectItem)
			: base(project, projectItem)
		{
		}
		
		internal override bool IsChildItem(SD.ProjectItem msbuildProjectItem)
		{
			string directory = Path.GetDirectoryName(msbuildProjectItem.Include);
			if (directory == relativePath) {
				return true;
			}
			return false;
		}
		
		public static DirectoryProjectItem CreateDirectoryProjectItemFromFullPath(Project project, string directory)
		{
			string relativePath = project.GetRelativePath(directory);
			return new DirectoryProjectItem(project, relativePath);
		}
	}
}
