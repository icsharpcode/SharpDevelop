// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter2 : CodeParameter, global::EnvDTE.CodeParameter2
	{
		public CodeParameter2(CodeModelContext context, IParameter parameter)
			: base(context, parameter)
		{
		}
		
		public virtual global::EnvDTE.vsCMParameterKind ParameterKind {
			get { return GetParameterKind(); }
		}
		
		global::EnvDTE.vsCMParameterKind GetParameterKind()
		{
			global::EnvDTE.vsCMParameterKind kind = 0;
			if (parameter.IsOptional) {
				kind |= global::EnvDTE.vsCMParameterKind.vsCMParameterKindOptional;
			}
			if (parameter.IsOut) {
				kind |= global::EnvDTE.vsCMParameterKind.vsCMParameterKindOut;
			} 
			if (parameter.IsRef) {
				kind |= global::EnvDTE.vsCMParameterKind.vsCMParameterKindRef;
			} 
			if (parameter.IsParams) {
				kind |= global::EnvDTE.vsCMParameterKind.vsCMParameterKindParamArray;
			} 
			if (!(parameter.IsOut || parameter.IsRef)) {
				kind |= global::EnvDTE.vsCMParameterKind.vsCMParameterKindIn;
			}
			return kind;
		}
	}
}
