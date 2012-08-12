// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter : CodeElement
	{
		IProjectContent projectContent;
		IParameter parameter;
		
		public CodeParameter(IProjectContent projectContent, IParameter parameter)
		{
			this.projectContent = projectContent;
			this.parameter = parameter;
		}
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementParameter; }
		}
		
		public override string Name {
			get { return parameter.Name; }
		}
		
		public virtual CodeTypeRef2 Type {
			get { return new CodeTypeRef2(projectContent, this, parameter.ReturnType); }
		}
	}
}
