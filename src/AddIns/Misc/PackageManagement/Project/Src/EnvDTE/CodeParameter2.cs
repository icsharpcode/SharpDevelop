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
			global::EnvDTE.vsCMParameterKind kind =  global::EnvDTE.vsCMParameterKind.vsCMParameterKindNone;
			if (parameter.IsOptional) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindOptional;
			}
			if (parameter.IsOut) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindOut;
			}
			if (parameter.IsRef) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindRef;
			}
			if (parameter.IsParams) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindParamArray;
			}
			return kind;
		}
	}
}
