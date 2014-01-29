// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeVariable : CodeElement, global::EnvDTE.CodeVariable
	{
		readonly IFieldModel field;
		
		public CodeVariable()
		{
		}
		
		public CodeVariable(CodeModelContext context, IFieldModel field)
			: base(context, field)
		{
			this.field = field;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementVariable; }
		}
		
		public global::EnvDTE.vsCMAccess Access {
			get { return field.Accessibility.ToAccess(); }
			set {
				var f = field.Resolve();
				if (f != null) {
					context.CodeGenerator.ChangeAccessibility(f, value.ToAccessibility());
				}
			}
		}
	}
}
