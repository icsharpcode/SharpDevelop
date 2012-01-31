// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectReferenceProjectItem : ReferenceProjectItem, IAssemblyReference
	{
		[Browsable(false)]
		public IProject ReferencedProject {
			get {
				// must be thread-safe because it's used by LoadSolutionProjectsThread
				return ProjectService.GetProject(this.FileName);
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
			this.DefaultCopyLocalValue = true;
		}
		
		IAssembly IAssemblyReference.Resolve(ITypeResolveContext context)
		{
			IProject p = this.ReferencedProject;
			if (p == null)
				return null;
			var snapshot = context.Compilation.SolutionSnapshot as SharpDevelopSolutionSnapshot;
			IProjectContent pc = (snapshot != null) ? snapshot.GetProjectContent(p) : p.ProjectContent;
			return (pc != null) ? pc.Resolve(context) : null;
		}
	}
}
