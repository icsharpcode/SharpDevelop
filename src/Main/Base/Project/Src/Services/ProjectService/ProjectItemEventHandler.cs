// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectItemEventArgs : ProjectEventArgs
	{
		ProjectItem projectItem;
		
		public ProjectItem ProjectItem {
			get {
				return projectItem;
			}
		}
		
		public ProjectItemEventArgs(IProject project, ProjectItem projectItem) : base(project)
		{
			this.projectItem = projectItem;
		}
	}
}
