// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
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
		
		public override string ShortName {
			get { return Path.GetFileNameWithoutExtension(Include); }
		}
		
		// hide Version,Culture,PublicKeyToken,SpecificVersion in property pad
		// (they are meaningless for project references)
		[Browsable(false)]
		public override Version Version {
			get { return null; }
		}
		
		[Browsable(false)]
		public override string Culture {
			get { return null; }
		}
		
		[Browsable(false)]
		public override string PublicKeyToken {
			get { return null; }
		}
		
		[Browsable(false)]
		public override bool SpecificVersion {
			get { return false; }
		}
		
		internal ProjectReferenceProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem buildItem)
			: base(project, buildItem)
		{
			this.DefaultCopyLocalValue = true;
		}
		
		public ProjectReferenceProjectItem(IProject project, IProject referenceTo)
			: base(project, ItemType.ProjectReference)
		{
			this.Include = FileUtility.GetRelativePath(project.Directory, referenceTo.FileName);
			ProjectGuid = referenceTo.IdGuid;
			ProjectName = referenceTo.Name;
			this.referencedProject = referenceTo;
			this.DefaultCopyLocalValue = true;
		}
	}
}
