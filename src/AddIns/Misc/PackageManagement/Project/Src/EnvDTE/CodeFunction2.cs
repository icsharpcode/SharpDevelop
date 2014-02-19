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
