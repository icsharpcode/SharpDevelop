// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcProjectRazorFile
	{
		public static MvcProjectFile CreateMvcProjectRazorFile(ProjectItem projectItem)
		{
			FileProjectItem fileProjectItem = projectItem as FileProjectItem;
			return CreateMvcProjectRazorFile(fileProjectItem);
		}
		
		public static MvcProjectFile CreateMvcProjectRazorFile(FileProjectItem fileProjectItem)
		{
			if (IsRazorFile(fileProjectItem)) {
				return new MvcProjectFile(fileProjectItem);
			}
			return null;
		}
		
		public static bool IsRazorFile(FileProjectItem fileProjectItem)
		{
			if (fileProjectItem != null) {
				return IsRazorFileName(fileProjectItem.FileName);
			}
			return false;
		}
		
		public static bool IsRazorFileName(string fileName)
		{
			string extension = MvcFileName.GetLowerCaseFileExtension(fileName);
			return extension == ".cshtml";
		}
	}
}
