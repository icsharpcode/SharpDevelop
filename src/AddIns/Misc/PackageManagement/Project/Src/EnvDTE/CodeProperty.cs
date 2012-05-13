// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeProperty : CodeElement
	{
		public CodeProperty()
		{
		}
		
		public virtual vsCMAccess Access { get; set; }
		
		public virtual CodeClass Parent {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Attributes {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeTypeRef Type {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeFunction Getter {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeFunction Setter {
			get { throw new NotImplementedException(); }
		}
	}
}
