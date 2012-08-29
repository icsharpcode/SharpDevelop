// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public static class FileProjectItemExtensions
	{
		public static bool IsDependentUponAnotherFile(this FileProjectItem projectItem)
		{
			return !String.IsNullOrEmpty(projectItem.DependentUpon);
		}
		
		public static bool IsDependentUpon(this FileProjectItem projectItem, FileProjectItem otherProjectItem)
		{
			return projectItem.DependentUpon == otherProjectItem.Include;
		}
		
		public static bool IsDependentUponFileName(this FileProjectItem projectItem, string fileName)
		{
			return FileUtility.IsEqualFileName(projectItem.GetDependentUponFileName(), fileName);
		}
		
		public static string GetDependentUponFileName(this FileProjectItem projectItem)
		{
			string directory = Path.GetDirectoryName(projectItem.FileName);
			return Path.Combine(directory, projectItem.DependentUpon);
		}
	}
}
