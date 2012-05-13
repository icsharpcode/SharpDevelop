// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeType : CodeElement
	{
		public virtual vsCMAccess Access { get; set; }
		
		public virtual string FullName {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Members {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Bases {
			get { throw new NotImplementedException(); }
		}
		
		public virtual CodeElements Attributes {
			get { throw new NotImplementedException(); }
		}
	}
}
