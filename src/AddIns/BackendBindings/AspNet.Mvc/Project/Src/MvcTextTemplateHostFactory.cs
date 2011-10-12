// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHostFactory : IMvcTextTemplateHostFactory
	{
		public IMvcTextTemplateHost CreateMvcTextTemplateHost(
			IMvcProject mvcProject,
			IMvcTextTemplateHostAppDomain appDomain)
		{
			var assemblyResolver = new MvcTextTemplateAssemblyResolver();
			var appDomainFactory = new CurrentAppDomainFactory();
			return appDomain.CreateMvcTextTemplateHost(appDomainFactory, assemblyResolver, appDomain.ApplicationBase);
		}
	}
}
