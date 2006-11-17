// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectReferenceProjectItem : ReferenceProjectItem
	{
		IProject referencedProject;
		
		[Browsable(false)]
		public IProject ReferencedProject {
			get {
				if (referencedProject == null)
					referencedProject = ProjectService.GetProject(this.FileName);
				return referencedProject;
			}
		}
		
		[ReadOnly(true)]
		public string ProjectGuid {
			get {
				return GetEvaluatedMetadata("Project");
			}
			set {
				SetEvaluatedMetadata("Project", value);
			}
		}
		
		[ReadOnly(true)]
		public string ProjectName {
			get {
				return GetEvaluatedMetadata("Name");
			}
			set {
				SetEvaluatedMetadata("Name", value);
			}
		}
		
		internal ProjectReferenceProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem buildItem)
			: base(project, buildItem)
		{
		}
		
		public ProjectReferenceProjectItem(IProject project, IProject referenceTo)
			: base(project, ItemType.ProjectReference)
		{
			this.Include = FileUtility.GetRelativePath(project.Directory, referenceTo.FileName);
			ProjectGuid = referenceTo.IdGuid;
			ProjectName = referenceTo.Name;
			this.referencedProject = referenceTo;
		}
	}
}
