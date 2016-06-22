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
