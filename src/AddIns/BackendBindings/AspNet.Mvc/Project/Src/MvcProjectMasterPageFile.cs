// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProjectMasterPageFile
	{
		public static MvcProjectFile CreateMvcProjectMasterPageFile(ProjectItem projectItem)
		{
			var fileProjectItem = projectItem as FileProjectItem;
			return CreateMvcProjectMasterPageFile(fileProjectItem);
		}
		
		public static MvcProjectFile CreateMvcProjectMasterPageFile(FileProjectItem fileProjectItem)
		{
			if (IsMasterPageFile(fileProjectItem)) {
				return new MvcProjectFile(fileProjectItem);
			}
			return null;
		}
		
		public static bool IsMasterPageFile(FileProjectItem fileProjectItem)
		{
			if (fileProjectItem != null) {
				return IsMasterPageFileName(fileProjectItem.FileName);
			}
			return false;
		}
		
		public static bool IsMasterPageFileName(string fileName)
		{
			string extension = MvcFileName.GetLowerCaseFileExtension(fileName);
			return extension == ".master";
		}
	}
}
