using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void ProjectReferenceEventHandler(object sender, ProjectReferenceEventArgs e);
	
	public class ProjectReferenceEventArgs 
	{
		IProject             project;
		ReferenceProjectItem referenceProjectItem;
		public IProject Project {
			get {
				return project;
			}
		}
		public ReferenceProjectItem ReferenceProjectItem {
			get {
				return referenceProjectItem;
			}
		}
		
		public ProjectReferenceEventArgs(IProject project, ReferenceProjectItem referenceProjectItem)
		{
			this.project = project;
			this.referenceProjectItem = referenceProjectItem;
		}
		
	}
	
}
