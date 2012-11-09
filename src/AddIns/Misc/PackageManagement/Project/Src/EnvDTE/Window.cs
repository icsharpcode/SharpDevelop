// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Window : MarshalByRefObject, global::EnvDTE.Window
	{
		public Window()
		{
		}
		
		public global::EnvDTE.Document Document {
			get { throw new NotImplementedException(); }
		}
	}
}
