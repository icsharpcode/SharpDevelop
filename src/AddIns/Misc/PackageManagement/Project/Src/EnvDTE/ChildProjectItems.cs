// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ChildProjectItems : EnumerableProjectItems
	{
		public ChildProjectItems(ProjectItem projectItem)
		{
			this.ProjectItem = projectItem;
			this.Project = (Project)projectItem.ContainingProject;
			this.ProjectItems = new List<ProjectItem>();
		}
		
		ProjectItem ProjectItem { get; set; }
		Project Project { get; set; }
		List<ProjectItem> ProjectItems { get; set; }
		
		protected override IEnumerable<global::EnvDTE.ProjectItem> GetProjectItems()
		{
			foreach (SD.ProjectItem msbuildProjectItem in Project.MSBuildProject.Items) {
				ProjectItem item = GetChildProjectItem(msbuildProjectItem);
				if (!IgnoreChildProjectItem(item)) {
					yield return item;
				}
			}
		}
		
		ProjectItem GetChildProjectItem(SD.ProjectItem msbuildProjectItem)
		{
			ProjectItemRelationship relationship = ProjectItem.GetRelationship(msbuildProjectItem);
			return relationship.GetChild();
		}
		
		bool IgnoreChildProjectItem(ProjectItem item)
		{
			if (item != null) {
				return SeenChildProjectItemBefore(item);
			}
			return true;
		}
		
		bool SeenChildProjectItemBefore(ProjectItem childProjectItem)
		{
			if (ProjectItems.Any(item => item.IsMatchByName(childProjectItem.Name))) {
				return true;
			}
			ProjectItems.Add(childProjectItem);
			return false;
		}
	}
}
