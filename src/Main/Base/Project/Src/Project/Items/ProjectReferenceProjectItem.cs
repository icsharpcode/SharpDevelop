// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectReferenceProjectItem : ReferenceProjectItem
	{
		public override ItemType ItemType {
			get {
				return ItemType.ProjectReference;
			}
		}
		
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
				return Properties["Project"];
			}
			set {
				Properties["Project"] = value;
			}
		}
		
		[ReadOnly(true)]
		public string ProjectName {
			get {
				return Properties["Name"];
			}
			set {
				Properties["Name"] = value;
			}
		}
		
		public ProjectReferenceProjectItem(IProject project) : base(project)
		{
		}
		
		public ProjectReferenceProjectItem(IProject project, IProject referenceTo) : base(project)
		{
			Include     = FileUtility.GetRelativePath(project.Directory, referenceTo.FileName);
			ProjectGuid = referenceTo.IdGuid;
			ProjectName = referenceTo.Name;
			this.referencedProject = referenceTo;
		}
		
		public override string ToString()
		{
			return String.Format("[ProjectReferenceProjectItem: Include={0}, Properties={1}]",
			                     Include,
			                     Properties);
		}
	}
}
