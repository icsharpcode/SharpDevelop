// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Reflection;

namespace ICSharpCode.TextTemplating
{
	[Serializable]
	public class TextTemplatingAppDomain : ITextTemplatingAppDomain
	{
		AppDomain appDomain;
		
		public TextTemplatingAppDomain(string applicationBase)
		{
			AppDomainSetup setupInfo = new AppDomainSetup();
			setupInfo.ApplicationBase = applicationBase;
			this.appDomain = AppDomain.CreateDomain("TextTemplatingAppDomain", null, setupInfo);
		}
		
		public AppDomain AppDomain {
			get { return this.appDomain; }
		}
		
		public void Dispose()
		{
			if (this.appDomain != null) {
				AppDomain.Unload(this.appDomain);
				this.appDomain = null;
			}
		}
	}
}
