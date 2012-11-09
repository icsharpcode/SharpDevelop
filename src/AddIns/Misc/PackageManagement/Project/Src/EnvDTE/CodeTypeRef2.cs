// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeRef2 : CodeTypeRef, global::EnvDTE.CodeTypeRef2
	{
		public CodeTypeRef2(IProjectContent projectContent, CodeElement parent, IReturnType returnType)
			: base(projectContent, parent, returnType)
		{
		}
		
		public bool IsGeneric {
			get { return ReturnType.DotNetName.Contains("{"); }
		}
	}
}
