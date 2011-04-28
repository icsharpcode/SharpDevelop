// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItemProperty : Property
	{
		ProjectItem projectItem;
		
		public ProjectItemProperty(ProjectItem projectItem, string name)
			: base(projectItem.ContainingProject, name)
		{
			this.projectItem = projectItem;
		}
		
		protected override object GetProperty()
		{
			return projectItem.GetProperty(Name);
		}
		
		protected override void SetProperty(object value)
		{
			projectItem.SetProperty(Name, value);
		}
	}
}
