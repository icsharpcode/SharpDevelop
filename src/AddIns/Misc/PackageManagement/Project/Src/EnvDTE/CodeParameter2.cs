// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter2 : CodeParameter, global::EnvDTE.CodeParameter2
	{
		public CodeParameter2(IProjectContent projectContent, IParameter parameter)
			: base(projectContent, parameter)
		{
		}
		
		public virtual global::EnvDTE.vsCMParameterKind ParameterKind {
			get { return GetParameterKind(); }
		}
		
		global::EnvDTE.vsCMParameterKind GetParameterKind()
		{
			if (Parameter.IsOptional) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindOptional;
			} else if (Parameter.IsOut) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindOut;
			} else if (Parameter.IsRef) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindRef;
			} else if (Parameter.IsParams) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindParamArray;
			} else if (Parameter.IsIn()) {
				return global::EnvDTE.vsCMParameterKind.vsCMParameterKindIn;
			}
			return global::EnvDTE.vsCMParameterKind.vsCMParameterKindNone;
		}
	}
}
