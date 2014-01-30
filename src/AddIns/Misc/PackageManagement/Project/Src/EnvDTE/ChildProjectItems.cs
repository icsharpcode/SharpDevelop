// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
