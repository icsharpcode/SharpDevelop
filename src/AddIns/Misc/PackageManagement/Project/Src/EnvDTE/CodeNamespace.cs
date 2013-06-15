// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeNamespace : CodeElement, global::EnvDTE.CodeNamespace
	{
		readonly INamespaceModel model;
		
		public CodeNamespace(CodeModelContext context, INamespaceModel model)
			: base(context, model)
		{
			this.model = model;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementNamespace; }
		}
		
		public override global::EnvDTE.vsCMInfoLocation InfoLocation {
			get { return global::EnvDTE.vsCMInfoLocation.vsCMInfoLocationExternal; }
		}
		
		public string FullName {
			get { return model.FullName; }
		}
		
		public virtual global::EnvDTE.CodeElements Members {
			get {
				IModelCollection<CodeElement> namespaceMembers = model.ChildNamespaces.Select(ns => new CodeNamespace(context, ns));
				IModelCollection<CodeElement> typeMembers = model.Types.Select(td => CodeType.Create(context, td));
				return namespaceMembers.Concat(typeMembers).AsCodeElements();
			}
		}
	}
}
