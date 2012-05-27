// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeVariable : CodeElement
	{
		public CodeVariable()
		{
		}
		
		public CodeVariable(IField field)
			: base(field)
		{
		}
		
		public vsCMAccess Access {
			get { return GetAccess(); }
			set { }
		}
	}
}
