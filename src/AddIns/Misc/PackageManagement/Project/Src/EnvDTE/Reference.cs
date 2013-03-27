// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Reference : MarshalByRefObject, global::EnvDTE.Reference
	{
		ReferenceProjectItem referenceProjectItem;
		Project project;
		
		public Reference(Project project, ReferenceProjectItem referenceProjectItem)
		{
			this.project = project;
			this.referenceProjectItem = referenceProjectItem;
		}
		
		public string Name {
			get { return referenceProjectItem.Name; }
		}
		
		public string Path {
			get { return referenceProjectItem.FileName; }
		}
		
		public void Remove()
		{
			project.RemoveReference(referenceProjectItem);
			project.Save();
		}
		
		public global::EnvDTE.Project SourceProject {
			get {
				var projectReference = referenceProjectItem as ProjectReferenceProjectItem;
				if (projectReference != null) {
					return new Project(projectReference.ReferencedProject as MSBuildBasedProject);
				}
				return null;
			}
		}
	}
}
