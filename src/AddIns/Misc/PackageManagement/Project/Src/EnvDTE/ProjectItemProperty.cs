// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class ProjectItemProperty : Property
	{
		ProjectItem projectItem;
		
		public ProjectItemProperty(ProjectItem projectItem, string name)
			: base(name)
		{
			this.projectItem = projectItem;
		}
		
		protected override object GetValue()
		{
			return projectItem.GetProperty(Name);
		}
		
		protected override void SetValue(object value)
		{
			projectItem.SetProperty(Name, value);
			projectItem.ContainingProject.Save();
		}
	}
}
