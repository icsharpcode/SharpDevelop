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
	public class CodeFunction : CodeElement, global::EnvDTE.CodeFunction
	{
		protected readonly IMethod method;
		
		public CodeFunction(CodeModelContext context, IMethod method)
			: base(context, method)
		{
			this.method = method;
		}
		
		public CodeFunction()
		{
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementFunction; }
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return method.Accessibility.ToAccess(); }
			set {
//				var method = method.Resolve();
//				if (method == null)
//					throw new NotSupportedException();
//				context.CodeGenerator.ChangeAccessibility(method, value.ToAccessibility());
			}
		}
		
		public virtual global::EnvDTE.CodeElements Parameters {
			get {
				var parameters = new CodeElementsList<CodeParameter2>();
				foreach (IParameter parameter in method.Parameters) {
					parameters.Add(new CodeParameter2(context, parameter));
				}
				return parameters;
			}
		}
		
		public virtual global::EnvDTE.CodeTypeRef2 Type {
			get {
				return new CodeTypeRef2(context, this, method.ReturnType);
			}
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return GetAttributes(method); }
		}
		
		public virtual bool CanOverride {
			get { return method.IsOverridable; }
			set {
				if (value && !method.IsOverridable) {
					context.CodeGenerator.MakeVirtual(method);
				}
			}
		}
		
		public virtual global::EnvDTE.vsCMFunction FunctionKind {
			get { return GetFunctionKind(); }
		}
		
		global::EnvDTE.vsCMFunction GetFunctionKind()
		{
			switch (method.SymbolKind) {
				case SymbolKind.Constructor:
					return global::EnvDTE.vsCMFunction.vsCMFunctionConstructor;
				//case SymbolKind.Destructor:
				//case SymbolKind.Accessor:
				//case SymbolKind.Operator:
				default:
					return global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
			}
		}
		
		public virtual bool IsShared {
			get { return method.IsStatic; }
		}
		
		public virtual bool MustImplement {
			get { return method.IsAbstract; }
		}
	}
}
