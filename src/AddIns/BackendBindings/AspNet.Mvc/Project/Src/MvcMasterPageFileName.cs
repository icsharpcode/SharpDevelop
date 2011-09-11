// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcMasterPageFileName
	{
		public MvcMasterPageFileName(FileProjectItem fileProjectItem)
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
		
		public MvcMasterPageFileName()
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
		
		public static MvcMasterPageFileName CreateMvcMasterPageFileName(ProjectItem projectItem)
		{
			var fileProjectItem = projectItem as FileProjectItem;
			if (fileProjectItem != null) {
				return CreateMvcMasterPageFileName(fileProjectItem);
			}
			return null;
		}
		
		public static MvcMasterPageFileName CreateMvcMasterPageFileName(FileProjectItem fileProjectItem)
		{
			if (IsMasterPageFileName(fileProjectItem.FileName)) {
				return new MvcMasterPageFileName(fileProjectItem);
			}
			return null;
		}
		
		public static bool IsMasterPageFileName(string fileName)
		{
			string extension = GetLowerCaseFileExtension(fileName);
			return extension == ".master";
		}
		
		static string GetLowerCaseFileExtension(string fileName)
		{
			if (fileName != null) {
				return Path.GetExtension(fileName).ToLowerInvariant();
			}
			return String.Empty;
		}
	}
}
