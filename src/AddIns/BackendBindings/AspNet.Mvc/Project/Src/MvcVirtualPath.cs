// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcVirtualPath
	{
		public MvcVirtualPath(string folderRelativeToProject, string fileName)
		{
			VirtualPath = GetVirtualPath(folderRelativeToProject, fileName);
		}
		
		public string VirtualPath { get; private set; }
		
		string GetVirtualPath(string folderRelativeToProject, string fileName)
		{
			return String.Format(
				"~/{0}{1}",
				GetFolderRelativeToProjectAsVirtualPath(folderRelativeToProject),
				fileName);
		}
		
		string GetFolderRelativeToProjectAsVirtualPath(string folderRelativeToProject)
		{
			string virtualFolder = folderRelativeToProject.Replace('\\', '/');
			return AppendForwardSlashIfVirtualFolderIsNotEmptyString(virtualFolder);
		}
		
		string AppendForwardSlashIfVirtualFolderIsNotEmptyString(string virtualFolder)
		{
			if (String.IsNullOrEmpty(virtualFolder)) {
				return String.Empty;
			}
			return virtualFolder + "/";
		}
	}
}
