// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class DirectoryProjectItems : ProjectItems
	{
		ProjectItem projectItem;
		
		public DirectoryProjectItems(ProjectItem projectItem)
			: base(projectItem.ContainingProject, new PackageManagementFileService())
		{
			this.projectItem = projectItem;
		}
		
		public override IEnumerator<ProjectItem> GetEnumerator()
		{
			var items = new ChildProjectItems(projectItem);
			return items.GetEnumerator();
		}
	}
}
