// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeType : CodeElement
	{
		CodeAttributes attributes;
		
		public CodeType(IProjectContent projectContent, IClass c)
			: base(c)
		{
			this.Class = c;
			InfoLocation = GetInfoLocation(projectContent, c);
		}
		
		vsCMInfoLocation GetInfoLocation(IProjectContent projectContent, IClass c)
		{
			if (projectContent.Project == c.ProjectContent.Project) {
				return vsCMInfoLocation.vsCMInfoLocationProject;
			}
			return vsCMInfoLocation.vsCMInfoLocationExternal;
		}
		
		public CodeType()
		{
		}
		
		protected IClass Class { get; private set; }
		
		public virtual vsCMAccess Access { get; set; }
		
		public virtual string FullName {
			get { return Class.FullyQualifiedName; }
		}
		
		public virtual CodeElements Members {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Bases {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Attributes {
			get {
				if (attributes == null) {
					attributes = new CodeAttributes(Class);
				}
				return attributes;
			}
		}
	}
}
