// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction2 : CodeFunction, global::EnvDTE.CodeFunction2
	{
		public CodeFunction2(CodeModelContext context, IMethod method)
			: base(context, method)
		{
		}
		
		public virtual bool IsGeneric {
			get { return method.TypeParameters.Count > 0; }
		}
		
		public virtual global::EnvDTE.vsCMOverrideKind OverrideKind {
			get { return GetOverrideKind(); }
		}
		
		global::EnvDTE.vsCMOverrideKind GetOverrideKind()
		{
			global::EnvDTE.vsCMOverrideKind kind = 0;
			if (method.IsAbstract)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindAbstract;
			if (method.IsOverride)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindOverride;
			if (method.IsVirtual && !method.IsAbstract && !method.IsOverride)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindVirtual;
			if (method.IsSealed)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindSealed;
			if (method.IsShadowing)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNew;
			return kind;
		}
	}
}
