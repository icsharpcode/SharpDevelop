// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction2 : CodeFunction, global::EnvDTE.CodeFunction2
	{
		public CodeFunction2(CodeModelContext context, IMethodModel methodModel)
			: base(context, methodModel)
		{
		}
		
		public virtual bool IsGeneric {
			get { return methodModel.TypeParameterCount > 0; }
		}
		
		public virtual global::EnvDTE.vsCMOverrideKind OverrideKind {
			get { return GetOverrideKind(); }
		}
		
		global::EnvDTE.vsCMOverrideKind GetOverrideKind()
		{
			global::EnvDTE.vsCMOverrideKind kind = 0;
			if (methodModel.IsAbstract)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindAbstract;
			if (methodModel.IsOverride)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindOverride;
			if (methodModel.IsVirtual && !methodModel.IsAbstract && !methodModel.IsOverride)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindVirtual;
			if (methodModel.IsSealed)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindSealed;
			if (methodModel.IsShadowing)
				kind |= global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNew;
			return kind;
		}
	}
}
