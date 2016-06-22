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
