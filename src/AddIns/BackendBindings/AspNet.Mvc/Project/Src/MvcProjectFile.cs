// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
