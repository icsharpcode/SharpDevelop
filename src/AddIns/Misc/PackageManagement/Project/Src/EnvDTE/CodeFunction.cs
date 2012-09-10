// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeFunction : CodeElement
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
		
		public override vsCMElement Kind {
			get { return vsCMElement.vsCMElementFunction; }
		}
		
		public virtual vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
		
		public override TextPoint GetStartPoint()
		{
			return new TextPoint(Method.GetStartPosition(), documentLoader);
		}
		
		public override TextPoint GetEndPoint()
		{
			return new TextPoint(Method.GetEndPosition(), documentLoader);
		}
		
		public virtual CodeElements Parameters {
			get { return new CodeParameters(Method.ProjectContent, Method.Parameters); }
		}
		
		public virtual CodeTypeRef2 Type {
			get { return new CodeTypeRef2(Method.ProjectContent, this, Method.ReturnType); }
		}
		
		public virtual CodeElements Attributes {
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
		
		public virtual vsCMFunction FunctionKind {
			get { return GetFunctionKind(); }
		}
		
		vsCMFunction GetFunctionKind()
		{
			if (Method.IsConstructor()) {
				return vsCMFunction.vsCMFunctionConstructor;
			}
			return vsCMFunction.vsCMFunctionFunction;
		}
		
		public virtual bool IsShared {
			get { return Method.IsStatic; }
		}
		
		public virtual bool MustImplement {
			get { return Method.IsAbstract; }
		}
	}
}
