// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class NullIISAdministrator : IISAdministrator
	{
		public NullIISAdministrator()
		{
		}
		
		public override bool IsIISInstalled()
		{
			return false;
		}
		
		public override void CreateVirtualDirectory(WebProject project)
		{
		}
	}
}
