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
