// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeTypeRef2 : CodeTypeRef, global::EnvDTE.CodeTypeRef2
	{
		public CodeTypeRef2(CodeModelContext context, CodeElement parent, IType type)
			: base(context, parent, type)
		{
		}
		
		public bool IsGeneric {
			get { return type.IsParameterized; }
		}
	}
}
