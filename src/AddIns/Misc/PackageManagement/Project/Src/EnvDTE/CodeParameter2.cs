// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter2 : CodeParameter
	{
		public CodeParameter2(IProjectContent projectContent, IParameter parameter)
			: base(projectContent, parameter)
		{
		}
		
		public virtual vsCMParameterKind ParameterKind {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Attributes {
			get { throw new NotImplementedException(); }
		}
	}
}
