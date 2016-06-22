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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProjectFile : IComparable<MvcProjectFile>
	{
		public MvcProjectFile(FileProjectItem fileProjectItem)
		{
			UpdateFileName(fileProjectItem);
		}
		
		void UpdateFileName(FileProjectItem fileProjectItem)
		{
			FullPath = fileProjectItem.FileName;
			FileName = Path.GetFileName(FullPath);
			FolderRelativeToProject = GetFolderRelativeToProject(fileProjectItem);
			VirtualPath = GetVirtualPath();
		}
		
		string GetFolderRelativeToProject(FileProjectItem fileProjectItem)
		{
			return Path.GetDirectoryName(fileProjectItem.Include);
		}
		
		string GetVirtualPath()
		{
			var virtualPath = new MvcVirtualPath(FolderRelativeToProject, FileName);
			return virtualPath.VirtualPath;
		}
		
		public MvcProjectFile()
		{
			FullPath = String.Empty;
			FileName = String.Empty;
			FolderRelativeToProject = String.Empty;
			VirtualPath = String.Empty;
		}
		
		public string FullPath { get; set; }
		public string FileName { get; set; }
		public string FolderRelativeToProject { get; set; }
		public string VirtualPath { get; set; }
		
		public override string ToString()
		{
			return FullPath;
		}
		
		static string GetLowerCaseFileExtension(string fileName)
		{
			if (fileName != null) {
				return Path.GetExtension(fileName).ToLowerInvariant();
			}
			return String.Empty;
		}
		
		public int CompareTo(MvcProjectFile other)
		{
			int result = CompareFileNames(other);
			if (result == 0) {
				return CompareFolderRelativeToProject(other);
			}
			return result;
		}
		
		int CompareFileNames(MvcProjectFile other)
		{
			return FileName.CompareTo(other.FileName);
		}
		
		int CompareFolderRelativeToProject(MvcProjectFile other)
		{
			return FolderRelativeToProject.CompareTo(other.FolderRelativeToProject);
		}
	}
}
