// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter : CodeElement, global::EnvDTE.CodeParameter
	{
		IProjectContent projectContent;
		
		public CodeParameter(IProjectContent projectContent, IParameter parameter)
		{
			this.projectContent = projectContent;
			this.Parameter = parameter;
		}
		
		protected IParameter Parameter { get; private set; }
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementParameter; }
		}
		
		public override string Name {
			get { return Parameter.Name; }
		}
		
		public virtual global::EnvDTE.CodeTypeRef2 Type {
			get { return new CodeTypeRef2(projectContent, this, Parameter.ReturnType); }
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return new CodeAttributes(Parameter); }
		}
	}
}
