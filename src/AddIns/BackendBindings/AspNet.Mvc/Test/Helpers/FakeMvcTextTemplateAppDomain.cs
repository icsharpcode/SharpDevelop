// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using ICSharpCode.TextTemplating;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcTextTemplateAppDomain : IMvcTextTemplateHostAppDomain
	{
		public string ApplicationBase { get; set; }
		public AppDomain AppDomain { get; set; }
		
		public IMvcTextTemplateHost CreateMvcTextTemplateHost(
			ITextTemplatingAppDomainFactory appDomainFactory, 
			ITextTemplatingAssemblyResolver assemblyResolver,
			string applicationBase)
		{
			return null;
		}
		
		public bool IsDisposed;
		
		public void Dispose()
		{
			IsDisposed = true;
		}
	}
}
