// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;

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
		
		public override IEnumerator GetEnumerator()
		{
			var items = new ChildProjectItems(projectItem);
			return items.GetEnumerator();
		}
	}
}
