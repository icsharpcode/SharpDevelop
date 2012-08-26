// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Document
	{
		public Document(string fileName)
		{
			this.FullName = fileName;
		}
		
		public virtual bool Saved {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		public string FullName { get; private set; }
	}
}
