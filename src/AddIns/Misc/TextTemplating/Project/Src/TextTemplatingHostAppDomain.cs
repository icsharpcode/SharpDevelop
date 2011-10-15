// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingHostAppDomain : IAppDomain
	{
		AppDomain appDomain;
		
		public TextTemplatingHostAppDomain()
		{
			this.appDomain = AppDomain.CurrentDomain;
		}
		
		public event ResolveEventHandler AssemblyResolve {
			add { appDomain.AssemblyResolve += value; }
			remove { appDomain.AssemblyResolve -= value; }
		}
	}
}
