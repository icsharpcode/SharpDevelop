// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
