// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItemsInsideDirectory : IEnumerable<ProjectItem>
	{
		Project project;
		Dictionary<string, string> directoriesIncluded = new Dictionary<string, string>();
		
		public ProjectItemsInsideDirectory(Project project)
		{
			this.project = project;
		}
		
		public IEnumerator<ProjectItem> GetEnumerator()
		{
			foreach (SD.ProjectItem item in project.MSBuildProject.Items) {
				ProjectItem projectItem = ConvertToProjectItem(item);
				if (projectItem != null) {
					yield return projectItem;
				}
			}
		}
		
		ProjectItem ConvertToProjectItem(SD.ProjectItem item)
		{
			var fileItem = item as SD.FileProjectItem;
			if (fileItem != null) {
				return ConvertFileToProjectItem(fileItem);
			}
			return null;
		}
		
		ProjectItem ConvertFileToProjectItem(SD.FileProjectItem fileItem)
		{
			if (IsInProjectRootFolder(fileItem)) {
				if (IsDirectory(fileItem)) {
					return CreateDirectoryProjectItemIfDirectoryNotAlreadyIncluded(fileItem);
				}
				return new ProjectItem(fileItem);
			}
			return ConvertDirectoryToProjectItem(fileItem);
		}
		
		bool IsInProjectRootFolder(SD.FileProjectItem item)
		{
			if (item.IsLink) {
				return !HasDirectoryInPath(item.VirtualName);
			}
			return !HasDirectoryInPath(item.Include);
		}
		
		bool HasDirectoryInPath(string path)
		{
			string directoryName = Path.GetDirectoryName(path);
			return !String.IsNullOrEmpty(directoryName);
		}
		
		bool IsDirectory(SD.FileProjectItem fileItem)
		{
			return fileItem.ItemType == SD.ItemType.Folder;
		}
		
		ProjectItem CreateDirectoryProjectItemIfDirectoryNotAlreadyIncluded(SD.FileProjectItem fileItem)
		{
			string directory = fileItem.Include;
			if (!IsDirectoryIncludedAlready(directory)) {
				AddIncludedDirectory(directory);
				return new ProjectItem(fileItem);
			}
			return null;
		}
		
		ProjectItem ConvertDirectoryToProjectItem(SD.FileProjectItem fileItem)
		{
			string subDirectoryName = GetFirstSubDirectoryName(fileItem.Include);
			if (IsDirectoryInsideProject(subDirectoryName)) {
				return CreateDirectoryProjectItemIfDirectoryNotAlreadyIncluded(subDirectoryName);
			}
			return null;
		}

		ProjectItem CreateDirectoryProjectItemIfDirectoryNotAlreadyIncluded(string subDirectoryName)
		{
			if (!IsDirectoryIncludedAlready(subDirectoryName)) {
				AddIncludedDirectory(subDirectoryName);
				return CreateDirectoryProjectItem(subDirectoryName);
			}
			return null;
		}
		
		bool IsDirectoryInsideProject(string directoryName)
		{
			return !directoryName.StartsWith("..");
		}
		
		bool IsDirectoryIncludedAlready(string directory)
		{
			return directoriesIncluded.ContainsKey(directory);
		}
		
		void AddIncludedDirectory(string directoryName)
		{
			directoriesIncluded.Add(directoryName, directoryName);
		}
		
		ProjectItem CreateDirectoryProjectItem(string directoryName)
		{
			var directoryItem = new SD.FileProjectItem(project.MSBuildProject, SD.ItemType.Folder);
			directoryItem.Include = directoryName;
			return new ProjectItem(directoryItem);
		}
		
		string GetFirstSubDirectoryName(string include)
		{
			string[] directoryNames = include.Split('\\');
			return directoryNames[0];
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
