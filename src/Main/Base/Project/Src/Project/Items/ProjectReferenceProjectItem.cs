// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

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
		
		[DefaultValue(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.ReferenceOutputAssembly}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.ReferenceOutputAssembly.Description}")]
		public bool ReferenceOutputAssembly {
			get {
				return GetEvaluatedMetadata("ReferenceOutputAssembly", true);
			}
			set {
				if (value)
					RemoveMetadata("ReferenceOutputAssembly");
				else
					SetEvaluatedMetadata("ReferenceOutputAssembly", "false");
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
		
		internal ProjectReferenceProjectItem(IProject project, IProjectItemBackendStore buildItem)
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
