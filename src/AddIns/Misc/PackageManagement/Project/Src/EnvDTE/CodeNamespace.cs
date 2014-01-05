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
}
