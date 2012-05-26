// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass2 : CodeClass
	{
		public CodeClass2(IProjectContent projectContent, IClass c)
			: base(projectContent, c)
		{
		}
		
		public CodeElements PartialClasses {
			get { throw new NotImplementedException(); }
		}
	}
}
