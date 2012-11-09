// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class TextDocument : MarshalByRefObject, global::EnvDTE.TextDocument
	{
		public TextDocument()
		{
		}
		
		public global::EnvDTE.TextPoint StartPoint {
			get { throw new NotImplementedException(); }
		}
	}
}
