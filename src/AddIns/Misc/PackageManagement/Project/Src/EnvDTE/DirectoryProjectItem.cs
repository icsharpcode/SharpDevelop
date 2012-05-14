// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
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
			string directory = GetLastDirectoryName(relativePath);
			return new FileProjectItem(project.MSBuildProject, ItemType.Folder, directory);
		}
		
		static string GetLastDirectoryName(string relativePath)
		{
			string[] directoryNames = relativePath.Split('\\');
			return directoryNames[1];
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
	}
}
