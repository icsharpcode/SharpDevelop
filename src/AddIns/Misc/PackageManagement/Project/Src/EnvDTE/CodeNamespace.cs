// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement, global::EnvDTE.CodeNamespace
	{
		INamespace ns;
		
		public CodeNamespace(CodeModelContext context, INamespace ns)
			: base(context)
		{
			this.ns = ns;
			this.InfoLocation = global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
			this.Language = context.CurrentProject.GetCodeModelLanguage();
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementNamespace; }
		}
		
		public string FullName {
			get { return ns.FullName; }
		}
		
		public override string Name {
			get { return ns.Name; }
		}
		
		public virtual global::EnvDTE.CodeElements Members {
			get { return new CodeElementsInNamespace(context, ns); }
		}
	}
	
	// Move code below into FileCodeModelNamespace
	public class CodeNamespaceBase : CodeElement, global::EnvDTE.CodeNamespace
	{
		NamespaceName namespaceName;
		
		public CodeNamespaceBase(CodeModelContext context, string qualifiedName)
			: this(context, new NamespaceName(qualifiedName))
		{
		}
		
		public CodeNamespaceBase(CodeModelContext context, NamespaceName namespaceName)
		{
			this.context = context;
			this.namespaceName = namespaceName;
			this.InfoLocation = global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal;
			this.Language = context.CurrentProject.GetCodeModelLanguage();
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementNamespace; }
		}
		
		internal NamespaceName NamespaceName {
			get { return namespaceName; }
		}
		
		public string FullName {
			get { return namespaceName.QualifiedName; }
		}
		
		public override string Name {
			get { return namespaceName.LastPart; }
		}
		
		public virtual global::EnvDTE.CodeElements Members {
			get { throw new NotImplementedException(); }
			//get { return new CodeElementsInNamespace(context, namespaceName); }
		}
		
//		CodeElementsList<CodeElement> members;
		
//		public virtual global::EnvDTE.CodeElements Members {
//			get {
//				if (members == null) {
//					if (model == null)
//						throw new NotSupportedException();
//					IModelCollection<CodeElement> namespaceMembers = model.ChildNamespaces.Select(ns => new CodeNamespace(context, ns));
//					IModelCollection<CodeElement> typeMembers = model.Types.Select(td => CodeType.Create(context, td));
//					members = namespaceMembers.Concat(typeMembers).AsCodeElements();
//				}
//				return members;
//			}
//		}
	}
}
