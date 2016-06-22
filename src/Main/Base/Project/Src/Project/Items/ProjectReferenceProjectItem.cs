// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectReferenceProjectItem : ReferenceProjectItem, IAssemblyReference
	{
		[Browsable(false)]
		public IProject ReferencedProject {
			get {
				// must be thread-safe because it's used by LoadSolutionProjectsThread,
				// and by IAssemblyReference.Resolve()
				IProject parentProject = this.Project;
				if (parentProject == null)
					return null;
				var fileName = this.FileName;
				foreach (var project in parentProject.ParentSolution.Projects) {
					if (project.FileName == fileName)
						return project;
				}
				return null;
			}
		}
		
		[ReadOnly(true)]
		public Guid ProjectGuid {
			get {
				Guid guid;
				if (Guid.TryParse(GetEvaluatedMetadata("Project"), out guid))
					return guid;
				else
					return Guid.Empty;
			}
			set {
				SetEvaluatedMetadata("Project", value.ToString("B"));
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
			this.DefaultCopyLocalValue = true;
		}
		
		IAssembly IAssemblyReference.Resolve(ITypeResolveContext context)
		{
			IProject p = this.ReferencedProject;
			if (p == null)
				return null;
			var snapshot = context.Compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			IProjectContent pc = (snapshot != null) ? snapshot.GetProjectContent(p) : p.ProjectContent;
			return (pc != null) ? pc.Resolve(context) : null;
		}
	}
}
