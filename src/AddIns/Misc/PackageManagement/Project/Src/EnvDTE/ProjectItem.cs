// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItem
	{
		SD.ProjectItem projectItem;
		
		public ProjectItem(SD.ProjectItem projectItem)
		{
			this.projectItem = projectItem;
		}
		
		public string Name {
			get { return Path.GetFileName(projectItem.Include); }
		}
	}
}
