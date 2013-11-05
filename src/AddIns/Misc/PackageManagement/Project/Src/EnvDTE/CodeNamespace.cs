// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement, global::EnvDTE.CodeNamespace
	{
		readonly string fullName;
		INamespaceModel model;
		
		public CodeNamespace(CodeModelContext context, INamespaceModel model)
			: base(context, model)
		{
			this.model = model;
		}
		
		public CodeNamespace(CodeModelContext context, string fullName)
			: base(context)
		{
			this.fullName = fullName;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementNamespace; }
		}
		
		public override global::EnvDTE.vsCMInfoLocation InfoLocation {
			get { return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal; }
		}
		
		public string FullName {
			get { return fullName; }
		}
		
		CodeElementsList<CodeElement> members;
		
		public virtual global::EnvDTE.CodeElements Members {
			get {
				if (members == null) {
					if (model == null)
						throw new NotSupportedException();
					IModelCollection<CodeElement> namespaceMembers = model.ChildNamespaces.Select(ns => new CodeNamespace(context, ns));
					IModelCollection<CodeElement> typeMembers = model.Types.Select(td => CodeType.Create(context, td));
					members = namespaceMembers.Concat(typeMembers).AsCodeElements();
				}
				return members;
			}
		}
	}
}
