// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeParameter : CodeElement, global::EnvDTE.CodeParameter
	{
		protected readonly IParameter parameter;
		
		public CodeParameter(CodeModelContext context, IParameter parameter)
			: base(context)
		{
			this.parameter = parameter;
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementParameter; }
		}
		
		public override string Name {
			get { return parameter.Name; }
		}
		
		public virtual global::EnvDTE.CodeTypeRef2 Type {
			get { return new CodeTypeRef2(context, this, parameter.Type); }
		}
		
		public virtual global::EnvDTE.CodeElements Attributes {
			get {
				var list = new CodeElementsList<CodeAttribute2>();
				foreach (var attr in parameter.Attributes) {
					list.Add(new CodeAttribute2(context, attr));
				}
				return list;
			}
		}
	}
}
