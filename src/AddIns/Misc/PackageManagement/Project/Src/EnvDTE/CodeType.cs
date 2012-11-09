// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeType : CodeElement, global::EnvDTE.CodeType
	{
		CodeTypeMembers members;
		
		/// <summary>
		/// Note that projectContent may be different to the IClass.ProjectContent since the class
		/// is retrieved from the namespace contents and could belong to a separate project or
		/// referenced assembly.
		/// </summary>
		public CodeType(IProjectContent projectContent, IClass c)
			: base(c)
		{
			this.Class = c;
			this.ProjectContent = projectContent;
			InfoLocation = GetInfoLocation(projectContent, c);
		}
		
		global::EnvDTE.vsCMInfoLocation GetInfoLocation(IProjectContent projectContent, IClass c)
		{
			if (projectContent.Project == c.ProjectContent.Project) {
				return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationProject;
			}
			return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
		}
		
		public CodeType()
		{
		}
		
		protected IClass Class { get; private set; }
		protected IProjectContent ProjectContent { get; private set; }
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public virtual string FullName {
			get { return Class.FullyQualifiedName; }
		}
		
		public virtual global::EnvDTE.CodeElements Members {
			get {
				if (members == null) {
					members = new CodeTypeMembers(ProjectContent, Class);
				}
				return members;
			}
		}
		
		public virtual global::EnvDTE.CodeElements Bases {
			get { return new CodeTypeBaseTypes(ProjectContent, Class); }
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return new CodeAttributes(Class); }
		}
		
		public virtual global::EnvDTE.CodeNamespace Namespace {
			get { return new FileCodeModelCodeNamespace(ProjectContent, Class.Namespace); }
		}
		
		public virtual global::EnvDTE.ProjectItem ProjectItem {
			get {
				if (ProjectContent.Project != null) {
					return new ProjectItem(ProjectContent, Class);
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns true if the current type matches the fully qualified name or any of its
		/// base types are a match.
		/// </summary>
		protected override bool GetIsDerivedFrom(string fullName)
		{
			return Class.IsDerivedFrom(fullName);
		}
	}
}
