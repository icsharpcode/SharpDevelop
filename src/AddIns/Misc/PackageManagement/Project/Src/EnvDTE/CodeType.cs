// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeType : CodeElement
	{
		public CodeType(IClass c)
			: base(c)
		{
			this.Class = c;
		}
		
		public CodeType()
		{
		}
		
		protected IClass Class { get; private set; }
		
		public virtual vsCMAccess Access { get; set; }
		
		public virtual string FullName {
			get { return Class.FullyQualifiedName; }
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
