// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction : CodeElement, global::EnvDTE.CodeFunction
	{
		IDocumentLoader documentLoader;
		IVirtualMethodUpdater methodUpdater;
		
		public CodeFunction(IMethod method)
			: this(method, new DocumentLoader(), new VirtualMethodUpdater(method))
		{
		}
		
		public CodeFunction(IMethod method, IDocumentLoader documentLoader, IVirtualMethodUpdater methodUpdater)
			: base(method)
		{
			this.Method = method;
			this.documentLoader = documentLoader;
			this.methodUpdater = methodUpdater;
		}
		
		public CodeFunction()
		{
		}
		
		public CodeFunction(IProperty property)
			: base(property)
		{
		}
		
		protected IMethodOrProperty Method { get; private set; }
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementFunction; }
		}
		
		public virtual global::EnvDTE.vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public override global::EnvDTE.TextPoint GetStartPoint()
		{
			return new TextPoint(Method.GetStartPosition(), documentLoader);
		}
		
		public override global::EnvDTE.TextPoint GetEndPoint()
		{
			return new TextPoint(Method.GetEndPosition(), documentLoader);
		}
		
		public virtual global::EnvDTE.CodeElements Parameters {
			get { return new CodeParameters(Method.ProjectContent, Method.Parameters); }
		}
		
		public virtual global::EnvDTE.CodeTypeRef2 Type {
			get { return new CodeTypeRef2(Method.ProjectContent, this, Method.ReturnType); }
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get { return new CodeAttributes(Method); }
		}
		
		public virtual bool CanOverride {
			get { return Method.IsOverridable; }
			set {
				if (value) {
					methodUpdater.MakeMethodVirtual();
				}
			}
		}
		
		public virtual global::EnvDTE.vsCMFunction FunctionKind {
			get { return GetFunctionKind(); }
		}
		
		global::EnvDTE.vsCMFunction GetFunctionKind()
		{
			if (Method.IsConstructor()) {
				return global::EnvDTE.vsCMFunction.vsCMFunctionConstructor;
			}
			return global::EnvDTE.vsCMFunction.vsCMFunctionFunction;
		}
		
		public virtual bool IsShared {
			get { return Method.IsStatic; }
		}
		
		public virtual bool MustImplement {
			get { return Method.IsAbstract; }
		}
	}
}
