// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeAttribute2 : CodeAttribute, global::EnvDTE.CodeAttribute2
	{
		public CodeAttribute2()
		{
		}
		
		public CodeAttribute2(CodeModelContext context, IAttribute attribute)
			: base(context, attribute)
		{
		}
		
		public virtual global::EnvDTE.CodeElements Arguments {
			get { 
				var list = new CodeElementsList<CodeAttributeArgument>();
				foreach (var arg in attribute.PositionalArguments) {
					list.Add(new CodeAttributeArgument(string.Empty, arg));
				}
				foreach (var arg in attribute.NamedArguments) {
					list.Add(new CodeAttributeArgument(arg.Key.Name, arg.Value));
				}
				return list;
			}
		}
	}
}
