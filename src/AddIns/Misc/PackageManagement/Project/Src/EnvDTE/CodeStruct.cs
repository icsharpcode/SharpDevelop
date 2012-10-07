// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeStruct : CodeType, global::EnvDTE.CodeStruct
	{
		public CodeStruct()
		{
		}
		
		public CodeStruct(IProjectContent projectContent, IClass c)
			: base(projectContent, c)
		{
		}
		
		public override global::EnvDTE.vsCMElement Kind {
			get { return global::EnvDTE.vsCMElement.vsCMElementStruct; }
		}
	}
}
