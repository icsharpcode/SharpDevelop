// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeStruct : CodeType, global::EnvDTE.CodeStruct
	{
		public CodeStruct()
		{
		}
		
		public CodeStruct(CodeModelContext context, ITypeDefinition typeDefinition)
			: base(context, typeDefinition)
		{
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementStruct; }
		}
	}
}
