// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeElementsInNamespace : CodeElementsList<CodeElement>
	{
		CodeModelContext context;
		INamespace ns;
		
		public CodeElementsInNamespace(CodeModelContext context)
			: this(context, context.DteProject.GetCompilationUnit().RootNamespace)
		{
		}
		
		public CodeElementsInNamespace(CodeModelContext context, INamespace ns)
		{
			this.context = context;
			this.ns = ns;
			GetCodeElements();
		}
		
		void GetCodeElements()
		{
			foreach (INamespace childNamespace in ns.ChildNamespaces) {
				AddCodeNamespace(childNamespace);
			}
			
			foreach (IType type in ns.Types) {
				AddType(type);
			}
		}
		
		void AddCodeNamespace(INamespace ns)
		{
			Add(new CodeNamespace(context, ns));
		}
		
		void AddType(IType type)
		{
			Add(CodeType.Create(context, type));
		}
	}
}
