// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Project
{
	public class BeforeBuildCustomToolFileNameFilter
	{
		List<string> fileNames;
		
		public BeforeBuildCustomToolFileNameFilter(IProject project)
		{
			var customToolOptions = new ProjectCustomToolOptions(project);
			if (customToolOptions.RunCustomToolOnBuild) {
				fileNames = customToolOptions.SplitFileNames().ToList();
			} else {
				fileNames = new List<string>();
			}
		}
		
		public bool IsMatch(string fullPath)
		{
			string fileNameToMatch = Path.GetFileName(fullPath);
			return fileNames.Any(fileName => String.Equals(fileName, fileNameToMatch, StringComparison.OrdinalIgnoreCase));
		}
		
		public bool Any()
		{
			return fileNames.Any();
		}
	}
}
