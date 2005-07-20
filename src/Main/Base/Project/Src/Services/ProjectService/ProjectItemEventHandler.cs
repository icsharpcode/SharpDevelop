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
