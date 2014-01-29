// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectCustomToolOptions
	{
		Properties properties;
		
		public ProjectCustomToolOptions(IProject project)
		{
			properties = project.Preferences.NestedProperties("customTool");
		}
		
		public bool RunCustomToolOnBuild {
			get { return properties.Get("runOnBuild", false); }
			set { properties.Set("runOnBuild", value); }
		}
		
		public string FileNames {
			get { return properties.Get("fileNames", String.Empty); }
			set { properties.Set("fileNames", value); }
		}
		
		public IList<string> SplitFileNames()
		{
			return
				FileNames
				.Replace("\r\n", ";")
				.Split(';', ',')
				.Select(fileName => fileName.Trim())
				.Where(fileName => !String.IsNullOrEmpty(fileName))
				.ToList();
		}
	}
}
