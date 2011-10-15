// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.SharpDevelop.Project;
using SD = ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ChildProjectItems : IEnumerable<ProjectItem>
	{
		public ChildProjectItems(ProjectItem projectItem)
		{
			this.ProjectItem = projectItem;
			this.Project = projectItem.ContainingProject;
		}
		
		ProjectItem ProjectItem { get; set; }
		Project Project { get; set; }
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public IEnumerator<ProjectItem> GetEnumerator()
		{
			List<ProjectItem> projectItems = GetProjectItems().ToList();
			return projectItems.GetEnumerator();
		}
		
		IEnumerable<ProjectItem> GetProjectItems()
		{
			foreach (SD.ProjectItem msbuildProjectItem in Project.MSBuildProject.Items) {
				ProjectItem item = ConvertToProjectItem(msbuildProjectItem);
				if (item != null) {
					yield return item;
				}
			}	
		}
		
		ProjectItem ConvertToProjectItem(SD.ProjectItem msbuildProjectItem)
		{
			ProjectItemRelationship relationship = ProjectItem.GetRelationship(msbuildProjectItem);
			return relationship.GetChild();
		}
	}
}
