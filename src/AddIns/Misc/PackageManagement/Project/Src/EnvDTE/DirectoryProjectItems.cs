// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DirectoryProjectItems : ProjectItems
	{
		ProjectItem projectItem;
		
		public DirectoryProjectItems(ProjectItem projectItem)
			: base(projectItem.ContainingProject, projectItem, new PackageManagementFileService())
		{
			this.projectItem = projectItem;
		}
		
		protected override IEnumerable<ProjectItem> GetProjectItems()
		{
			return new ChildProjectItems(projectItem);
		}
	}
}
