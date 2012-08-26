// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction2 : CodeFunction
	{
		public CodeFunction2(IMethod method)
			: base(method)
		{
		}
		
		public virtual bool IsGeneric {
			get { return Method.HasTypeParameters(); }
		}
		
		public virtual vsCMOverrideKind OverrideKind {
			get { return GetOverrideKind(); }
		}
		
		vsCMOverrideKind GetOverrideKind()
		{
			if (Method.IsAbstract) {
				return vsCMOverrideKind.vsCMOverrideKindAbstract;
			} else if (Method.IsVirtual) {
				return vsCMOverrideKind.vsCMOverrideKindVirtual;
			} else if (Method.IsOverride) {
				return vsCMOverrideKind.vsCMOverrideKindOverride;
			} else if (Method.IsSealed) {
				return vsCMOverrideKind.vsCMOverrideKindSealed;
			} else if (Method.IsNew) {
				return vsCMOverrideKind.vsCMOverrideKindNew;
			}
			return vsCMOverrideKind.vsCMOverrideKindNone;
		}
	}
}
