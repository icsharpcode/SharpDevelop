// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction : CodeElement, global::EnvDTE.CodeFunction
	{
		protected readonly IMethodModel methodModel;
		
		public CodeFunction(CodeModelContext context, IMethodModel methodModel)
			: base(context, methodModel)
		{
		}
		
		public CodeFunction()
		{
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementFunction; }
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return methodModel.Accessibility.ToAccess(); }
			set {
				var method = methodModel.Resolve();
				if (method == null)
					throw new NotSupportedException();
				context.CodeGenerator.ChangeAccessibility(method, value.ToAccessibility());
			}
		}
		
		public virtual global::EnvDTE.CodeElements Parameters {
			get {
				var list = new CodeElementsList<CodeParameter2>();
				var method = (IParameterizedMember)methodModel.Resolve();
				if (method != null) {
					foreach (var p in method.Parameters) {
						list.Add(new CodeParameter2(context, p));
					}
				}
				return list;
			}
		}
		
		public virtual global::EnvDTE.CodeTypeRef2 Type {
			get {
				var method = methodModel.Resolve();
				if (method == null)
					return null;
				return new CodeTypeRef2(context, this, method.ReturnType);
			}
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return GetAttributes(methodModel); }
		}
		
		public virtual bool CanOverride {
			get { return methodModel.IsOverridable; }
			set {
				if (value && !methodModel.IsOverridable) {
					var method = methodModel.Resolve();
					if (method != null) {
						context.CodeGenerator.MakeVirtual(method);
					}
				}
			}
		}
		
		public virtual global::EnvDTE.vsCMFunction FunctionKind {
			get { return GetFunctionKind(); }
		}
		
		global::EnvDTE.vsCMFunction GetFunctionKind()
		{
			switch (methodModel.SymbolKind) {
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
			get { return methodModel.IsStatic; }
		}
		
		public virtual bool MustImplement {
			get { return methodModel.IsAbstract; }
		}
	}
}
