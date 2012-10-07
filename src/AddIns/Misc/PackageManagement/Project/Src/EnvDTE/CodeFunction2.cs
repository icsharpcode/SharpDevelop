// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction2 : CodeFunction, global::EnvDTE.CodeFunction2
	{
		public CodeFunction2(IMethod method)
			: base(method)
		{
		}
		
		public virtual bool IsGeneric {
			get { return Method.HasTypeParameters(); }
		}
		
		public virtual global::EnvDTE.vsCMOverrideKind OverrideKind {
			get { return GetOverrideKind(); }
		}
		
		global::EnvDTE.vsCMOverrideKind GetOverrideKind()
		{
			if (Method.IsAbstract) {
				return global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindAbstract;
			} else if (Method.IsVirtual) {
				return global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindVirtual;
			} else if (Method.IsOverride) {
				return global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindOverride;
			} else if (Method.IsSealed) {
				return global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindSealed;
			} else if (Method.IsNew) {
				return global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNew;
			}
			return global::EnvDTE.vsCMOverrideKind.vsCMOverrideKindNone;
		}
	}
}
