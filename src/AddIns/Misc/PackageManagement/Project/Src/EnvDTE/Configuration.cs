// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Configuration : MarshalByRefObject
	{
		public Configuration()
		{
		}
		
		public Properties Properties {
			get { throw new NotImplementedException(); }
		}
	}
}
